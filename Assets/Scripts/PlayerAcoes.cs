using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAcoes : MonoBehaviour
{
    public static event Action<string> OnPromptInteracaoAlterado;

    [Header("Interação (Tecla E)")]
    [SerializeField] private float _raioInteracao = 1.6f;

    [Header("Ataque / Mineração (Espaço ou Clique Esquerdo)")]
    [SerializeField] private float _cooldownAtaque = 0.35f;
    [SerializeField] private float _distanciaAtaque = 1.0f;
    [SerializeField] private float _raioAtaque = 0.85f;
    [SerializeField] private int _danoMaos = 1;
    [SerializeField] private int _danoPicareta = 2;

    [Header("Feedback Visual")]
    [SerializeField] private Transform _visualFerramenta;
    [SerializeField] private float _duracaoSwing = 0.15f;

    private PlayerMovement _playerMovement;
    private PlayerVida _playerVida;
    private SpriteRenderer _srFerramenta;
    private float _proximoAtaquePermitido;
    private bool _emSwing;
    private string _ultimoPrompt = string.Empty;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerVida = GetComponent<PlayerVida>();
        if (_visualFerramenta == null)
        {
            Transform filho = transform.Find("VisualFerramenta");
            if (filho != null) _visualFerramenta = filho;
        }
        if (_visualFerramenta != null)
        {
            _srFerramenta = _visualFerramenta.GetComponent<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        InventarioAlquimista.OnFerramentaAlterada += AtualizarVisualFerramenta;
    }

    private void OnDisable()
    {
        InventarioAlquimista.OnFerramentaAlterada -= AtualizarVisualFerramenta;
    }

    private void Start()
    {
        AtualizarVisualFerramenta();
    }

    private void Update()
    {
        if (Time.timeScale <= 0f) return;
        if (_playerVida != null && _playerVida.EstaMorto) return;

        AtualizarPromptProximidade();
        AtualizarPosicaoFerramentaIdle();

        Keyboard teclado = Keyboard.current;
        Mouse mouse = Mouse.current;

        if (teclado != null && teclado.eKey.wasPressedThisFrame)
        {
            TentarInteragir();
        }

        if (teclado != null && teclado.fKey.wasPressedThisFrame)
        {
            TentarCurarComFruta();
        }

        bool pediuAtacar =
            (teclado != null && teclado.spaceKey.wasPressedThisFrame) ||
            (mouse != null && mouse.leftButton.wasPressedThisFrame);

        if (pediuAtacar && Time.time >= _proximoAtaquePermitido)
        {
            _proximoAtaquePermitido = Time.time + _cooldownAtaque;
            ExecutarAtaque();
        }
    }

    private void TentarCurarComFruta()
    {
        if (_playerVida == null) _playerVida = GetComponent<PlayerVida>();
        if (_playerVida == null || _playerVida.EstaMorto) return;

        if (_playerVida.EstaVidaCheia)
        {
            RecursoMineravel.EmitirAviso("Sua vida já está cheia!");
            return;
        }

        InventarioAlquimista inventario = InventarioAlquimista.Instance;
        if (inventario != null && inventario.TentarConsumir(TipoIngrediente.Fruta, 1))
        {
            _playerVida.RecuperarVida(1);
            RecursoMineravel.EmitirAviso("Você comeu 1 Fruta e recuperou +1 de Vida!");
        }
        else
        {
            RecursoMineravel.EmitirAviso("Sem Frutas na mochila para recuperar vida!");
        }
    }

    private void TentarInteragir()
    {
        IInteragivel maisProximo = BuscarInteragivelMaisProximo();
        if (maisProximo != null)
        {
            maisProximo.Interagir(this);
            AtualizarPromptProximidade(true);
        }
    }

    private IInteragivel BuscarInteragivelMaisProximo()
    {
        Collider2D[] colisores = Physics2D.OverlapCircleAll(transform.position, _raioInteracao);
        IInteragivel melhor = null;
        float menorDistancia = float.MaxValue;

        for (int i = 0; i < colisores.Length; i++)
        {
            IInteragivel interagivel = colisores[i].GetComponent<IInteragivel>();
            if (interagivel == null) continue;

            float dist = Vector2.Distance(transform.position, colisores[i].transform.position);
            if (dist < menorDistancia)
            {
                menorDistancia = dist;
                melhor = interagivel;
            }
        }

        return melhor;
    }

    private void AtualizarPromptProximidade(bool forcarNotificacao = false)
    {
        IInteragivel proximo = BuscarInteragivelMaisProximo();
        string novoPrompt = proximo != null ? proximo.ObterPromptInteracao() : string.Empty;

        if (forcarNotificacao || novoPrompt != _ultimoPrompt)
        {
            _ultimoPrompt = novoPrompt;
            OnPromptInteracaoAlterado?.Invoke(_ultimoPrompt);
        }
    }

    private void ExecutarAtaque()
    {
        Vector2 direcao = _playerMovement != null && _playerMovement.UltimaDirecao.sqrMagnitude > 0.001f
            ? _playerMovement.UltimaDirecao.normalized
            : Vector2.down;

        Vector2 centroGolpe = (Vector2)transform.position + (direcao * _distanciaAtaque);

        bool temPicareta = InventarioAlquimista.Instance != null && InventarioAlquimista.Instance.TemPicareta;
        int dano = temPicareta ? _danoPicareta : _danoMaos;

        Collider2D[] atingidos = Physics2D.OverlapCircleAll(centroGolpe, _raioAtaque);
        for (int i = 0; i < atingidos.Length; i++)
        {
            RecursoMineravel recurso = atingidos[i].GetComponent<RecursoMineravel>();
            if (recurso != null)
            {
                recurso.ReceberGolpe(dano, temPicareta);
            }

            InimigoElemental inimigo = atingidos[i].GetComponent<InimigoElemental>();
            if (inimigo != null)
            {
                Vector2 dirEmpurrao = ((Vector2)inimigo.transform.position - (Vector2)transform.position);
                if (dirEmpurrao.sqrMagnitude < 0.001f) dirEmpurrao = direcao;
                inimigo.ReceberDano(dano, dirEmpurrao.normalized);
            }
        }

        if (_visualFerramenta != null)
        {
            StopAllCoroutines();
            StartCoroutine(RotinaSwingGolpe(direcao, temPicareta));
        }
    }

    private void AtualizarVisualFerramenta()
    {
        if (_visualFerramenta == null) return;

        bool temPicareta = InventarioAlquimista.Instance != null && InventarioAlquimista.Instance.TemPicareta;
        if (!_emSwing)
        {
            _visualFerramenta.gameObject.SetActive(temPicareta);
        }

        if (_srFerramenta != null)
        {
            _srFerramenta.color = temPicareta
                ? new Color(0.75f, 0.82f, 0.90f, 1f)
                : new Color(1f, 1f, 1f, 0.75f);
        }
    }

    private void AtualizarPosicaoFerramentaIdle()
    {
        if (_visualFerramenta == null || _emSwing) return;

        bool temPicareta = InventarioAlquimista.Instance != null && InventarioAlquimista.Instance.TemPicareta;
        if (!temPicareta) return;

        Vector2 dir = _playerMovement != null ? _playerMovement.UltimaDirecao : Vector2.right;
        float ladoX = dir.x < -0.01f ? -0.55f : 0.55f;
        _visualFerramenta.localPosition = new Vector3(ladoX, -0.05f, 0f);
        _visualFerramenta.localRotation = Quaternion.Euler(0f, 0f, ladoX < 0f ? 35f : -35f);
    }

    private IEnumerator RotinaSwingGolpe(Vector2 direcao, bool temPicareta)
    {
        _emSwing = true;
        _visualFerramenta.gameObject.SetActive(true);

        if (_srFerramenta != null)
        {
            _srFerramenta.color = temPicareta
                ? new Color(0.82f, 0.92f, 1f, 1f)
                : new Color(1f, 1f, 1f, 0.8f);
        }

        float anguloBase = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        float anguloInicio = anguloBase + 55f;
        float anguloFim = anguloBase - 55f;

        float t = 0f;
        while (t < _duracaoSwing)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / _duracaoSwing);
            float anguloAtual = Mathf.Lerp(anguloInicio, anguloFim, p);
            float rad = anguloAtual * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * (_distanciaAtaque * 0.85f);

            _visualFerramenta.localPosition = offset;
            _visualFerramenta.localRotation = Quaternion.Euler(0f, 0f, anguloAtual - 90f);
            yield return null;
        }

        _emSwing = false;
        AtualizarVisualFerramenta();
        AtualizarPosicaoFerramentaIdle();
    }
}

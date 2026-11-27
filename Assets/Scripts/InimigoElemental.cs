using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class InimigoElemental : MonoBehaviour
{
    [Header("Atributos do Inimigo")]
    [SerializeField] private int _vidaMaxima = 3;
    [SerializeField] private float _velocidade = 2.2f;
    [SerializeField] private int _dano = 1;
    [SerializeField] private float _raioDeteccao = 6f;
    [SerializeField] private float _tempoRecargaAtaque = 1.2f;
    [SerializeField] private float _forcaKnockback = 4f;
    [SerializeField] private GameObject _prefabDrop;

    private int _vidaAtual;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Color _corOriginal;
    private Transform _alvoPlayer;
    private Vector2 _posicaoInicial;
    private Vector2 _pontoPatrulha;
    private float _proximoAtaquePermitido;
    private float _tempoFimKnockback;
    private float _proximaTrocaPatrulha;
    private Coroutine _rotinaPisca;

    private void Awake()
    {
        _vidaAtual = Mathf.Max(1, _vidaMaxima);
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _corOriginal = _spriteRenderer.color;
        }
        _posicaoInicial = transform.position;
        _pontoPatrulha = _posicaoInicial;
    }

    private void Start()
    {
        _posicaoInicial = transform.position;
        EscolherNovoPontoPatrulha();
        LocalizarPlayer();
    }

    private void FixedUpdate()
    {
        if (Time.time < _tempoFimKnockback)
        {
            _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, Vector2.zero, 10f * Time.fixedDeltaTime);
            return;
        }

        if (_alvoPlayer == null)
        {
            LocalizarPlayer();
        }

        if (_alvoPlayer != null)
        {
            float distancia = Vector2.Distance(transform.position, _alvoPlayer.position);
            if (distancia <= _raioDeteccao)
            {
                Vector2 direcao = ((Vector2)_alvoPlayer.position - (Vector2)transform.position).normalized;
                _rb.linearVelocity = direcao * _velocidade;
                AtualizarFlip(direcao.x);
                return;
            }
        }

        ExecutarPatrulhaSimples();
    }

    private void ExecutarPatrulhaSimples()
    {
        if (Time.time >= _proximaTrocaPatrulha)
        {
            EscolherNovoPontoPatrulha();
        }

        Vector2 diferenca = _pontoPatrulha - (Vector2)transform.position;
        if (diferenca.sqrMagnitude <= 0.08f)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = diferenca.normalized;
        _rb.linearVelocity = dir * (_velocidade * 0.45f);
        AtualizarFlip(dir.x);
    }

    private void EscolherNovoPontoPatrulha()
    {
        _pontoPatrulha = _posicaoInicial + (Random.insideUnitCircle * 1.4f);
        _proximaTrocaPatrulha = Time.time + Random.Range(2.2f, 3.8f);
    }

    private void AtualizarFlip(float dirX)
    {
        if (_spriteRenderer == null) return;
        if (dirX < -0.05f) _spriteRenderer.flipX = true;
        else if (dirX > 0.05f) _spriteRenderer.flipX = false;
    }

    private void LocalizarPlayer()
    {
        if (PlayerVida.Instance != null)
        {
            _alvoPlayer = PlayerVida.Instance.transform;
            return;
        }

        PlayerMovement pm = FindAnyObjectByType<PlayerMovement>();
        if (pm != null)
        {
            _alvoPlayer = pm.transform;
        }
    }

    public void ReceberDano(int dano, Vector2 direcaoEmpurrao)
    {
        if (_vidaAtual <= 0) return;

        _vidaAtual -= Mathf.Max(1, dano);

        Vector2 dirKnock = direcaoEmpurrao.sqrMagnitude > 0.001f
            ? direcaoEmpurrao.normalized
            : Vector2.up;

        _rb.linearVelocity = dirKnock * _forcaKnockback;
        _tempoFimKnockback = Time.time + 0.18f;

        if (_vidaAtual <= 0)
        {
            if (_prefabDrop != null)
            {
                Instantiate(_prefabDrop, transform.position, _prefabDrop.transform.rotation);
            }
            Destroy(gameObject);
            return;
        }

        if (_rotinaPisca != null)
        {
            StopCoroutine(_rotinaPisca);
        }
        _rotinaPisca = StartCoroutine(RotinaPiscarDano());
    }

    private IEnumerator RotinaPiscarDano()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = Color.white;
        }
        yield return new WaitForSeconds(0.07f);

        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = new Color(1f, 0.2f, 0.2f, 1f);
        }
        yield return new WaitForSeconds(0.07f);

        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _corOriginal;
        }
        _rotinaPisca = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TentarCausarDanoAoPlayer(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TentarCausarDanoAoPlayer(collision.gameObject);
    }

    private void TentarCausarDanoAoPlayer(GameObject objeto)
    {
        if (Time.time < _proximoAtaquePermitido) return;

        PlayerVida playerVida = objeto.GetComponent<PlayerVida>();
        if (playerVida == null && objeto.CompareTag("Player"))
        {
            playerVida = PlayerVida.Instance;
        }

        if (playerVida != null && !playerVida.EstaMorto)
        {
            _proximoAtaquePermitido = Time.time + _tempoRecargaAtaque;
            playerVida.TomarDano(_dano);
        }
    }
}

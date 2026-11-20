using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class RecursoMineravel : MonoBehaviour
{
    public static event Action<string> OnAvisoJogador;

    [SerializeField] private int _vidaMaxima = 3;
    [SerializeField] private bool _precisaPicareta = false;
    [SerializeField] private GameObject _prefabDrop;
    [SerializeField] private int _quantidadeDrop = 2;
    [SerializeField] private string _nomeRecurso = "Recurso";

    private int _vidaAtual;
    private SpriteRenderer _spriteRenderer;
    private Color _corOriginal;
    private Vector3 _posicaoInicial;
    private Coroutine _rotinaFeedback;

    public string NomeRecurso => _nomeRecurso;
    public bool PrecisaPicareta => _precisaPicareta;

    public static void EmitirAviso(string mensagem)
    {
        OnAvisoJogador?.Invoke(mensagem);
    }

    private void Awake()
    {
        _vidaAtual = Mathf.Max(1, _vidaMaxima);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _corOriginal = _spriteRenderer.color;
        }
        _posicaoInicial = transform.position;
    }

    private void Start()
    {
        _posicaoInicial = transform.position;
    }

    public void ReceberGolpe(int dano, bool jogadorTemPicareta)
    {
        if (_precisaPicareta && !jogadorTemPicareta)
        {
            AcionarFeedback(false);
            OnAvisoJogador?.Invoke("Precisa de Picareta! Crie na Bancada (3 Madeiras, 2 Pedras)");
            return;
        }

        _vidaAtual -= Mathf.Max(1, dano);

        if (_vidaAtual <= 0)
        {
            DroparRecursos();
            Destroy(gameObject);
            return;
        }

        AcionarFeedback(true);
    }

    private void DroparRecursos()
    {
        if (_prefabDrop == null) return;

        for (int i = 0; i < _quantidadeDrop; i++)
        {
            Vector2 offset = UnityEngine.Random.insideUnitCircle * 0.55f;
            Vector3 posDrop = _posicaoInicial + new Vector3(offset.x, offset.y, 0f);
            Instantiate(_prefabDrop, posDrop, _prefabDrop.transform.rotation);
        }
    }

    private void AcionarFeedback(bool piscarBranco)
    {
        if (_rotinaFeedback != null)
        {
            StopCoroutine(_rotinaFeedback);
            transform.position = _posicaoInicial;
            if (_spriteRenderer != null) _spriteRenderer.color = _corOriginal;
        }
        _rotinaFeedback = StartCoroutine(RotinaTremorEPiscar(piscarBranco));
    }

    private IEnumerator RotinaTremorEPiscar(bool piscarBranco)
    {
        float duracao = 0.1f;
        float t = 0f;

        if (piscarBranco && _spriteRenderer != null)
        {
            _spriteRenderer.color = Color.white;
        }

        while (t < duracao)
        {
            t += Time.deltaTime;
            Vector2 shake = UnityEngine.Random.insideUnitCircle * 0.08f;
            transform.position = _posicaoInicial + new Vector3(shake.x, shake.y, 0f);
            yield return null;
        }

        transform.position = _posicaoInicial;
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _corOriginal;
        }
        _rotinaFeedback = null;
    }
}

using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Caldeirao : MonoBehaviour
{
    public static Caldeirao Instance { get; private set; }

    public static event Action OnReceitaAlterada;
    public static event Action OnReceitaCompleta;

    [Header("Receita da Ilha")]
    [SerializeField] private int _metaFrutas = 3;
    [SerializeField] private int _metaCristais = 2;
    [SerializeField] private int _metaRaizes = 0;

    [Header("Feedback Visual")]
    [SerializeField] private Color _corEntrega = new Color(0.75f, 0.45f, 1f, 1f);
    [SerializeField] private Color _corCompleta = new Color(1f, 0.85f, 0.25f, 1f);
    [SerializeField] private float _duracaoPulso = 0.35f;

    private int _frutasEntregues;
    private int _cristaisEntregues;
    private int _raizesEntregues;
    private bool _receitaConcluida;

    private SpriteRenderer _spriteRenderer;
    private Color _corOriginal;
    private Vector3 _escalaOriginal;
    private Coroutine _rotinaFeedback;

    public int FrutasEntregues => _frutasEntregues;
    public int MetaFrutas => _metaFrutas;
    public int CristaisEntregues => _cristaisEntregues;
    public int MetaCristais => _metaCristais;
    public int RaizesEntregues => _raizesEntregues;
    public int MetaRaizes => _metaRaizes;
    public bool ReceitaConcluida => _receitaConcluida;

    private void Awake()
    {
        Instance = this;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _corOriginal = _spriteRenderer.color;
        }
        _escalaOriginal = transform.localScale;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_receitaConcluida) return;

        bool ehPlayer = other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null;
        if (!ehPlayer) return;

        InventarioAlquimista inventario = InventarioAlquimista.Instance != null
            ? InventarioAlquimista.Instance
            : other.GetComponent<InventarioAlquimista>();

        if (inventario == null) return;

        bool entregouAlgo = false;

        int faltaFrutas = Mathf.Max(0, _metaFrutas - _frutasEntregues);
        if (faltaFrutas > 0)
        {
            int entregue = inventario.ConsumirAte(TipoIngrediente.Fruta, faltaFrutas);
            if (entregue > 0)
            {
                _frutasEntregues += entregue;
                entregouAlgo = true;
            }
        }

        int faltaCristais = Mathf.Max(0, _metaCristais - _cristaisEntregues);
        if (faltaCristais > 0)
        {
            int entregue = inventario.ConsumirAte(TipoIngrediente.Cristal, faltaCristais);
            if (entregue > 0)
            {
                _cristaisEntregues += entregue;
                entregouAlgo = true;
            }
        }

        int faltaRaizes = Mathf.Max(0, _metaRaizes - _raizesEntregues);
        if (faltaRaizes > 0)
        {
            int entregue = inventario.ConsumirAte(TipoIngrediente.Raiz, faltaRaizes);
            if (entregue > 0)
            {
                _raizesEntregues += entregue;
                entregouAlgo = true;
            }
        }

        if (!entregouAlgo) return;

        VerificarConclusao();
        OnReceitaAlterada?.Invoke();

        if (_receitaConcluida)
        {
            AcionarFeedbackVisual(_corCompleta, 1.3f, true);
            Debug.Log("RECEITA COMPLETA! Ilha purificada!");
            OnReceitaCompleta?.Invoke();
        }
        else
        {
            AcionarFeedbackVisual(_corEntrega, 1.18f, false);
        }
    }

    private void VerificarConclusao()
    {
        _receitaConcluida =
            _frutasEntregues >= _metaFrutas &&
            _cristaisEntregues >= _metaCristais &&
            _raizesEntregues >= _metaRaizes;
    }

    private void AcionarFeedbackVisual(Color corAlvo, float multiplicadorEscala, bool manterCorFinal)
    {
        if (_rotinaFeedback != null)
        {
            StopCoroutine(_rotinaFeedback);
        }
        _rotinaFeedback = StartCoroutine(RotinaPulso(corAlvo, multiplicadorEscala, manterCorFinal));
    }

    private IEnumerator RotinaPulso(Color corAlvo, float multiplicadorEscala, bool manterCorFinal)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = corAlvo;
        }

        Vector3 escalaPico = _escalaOriginal * multiplicadorEscala;
        float metade = _duracaoPulso * 0.5f;
        float t = 0f;

        while (t < metade)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(_escalaOriginal, escalaPico, t / metade);
            yield return null;
        }

        t = 0f;
        while (t < metade)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(escalaPico, _escalaOriginal, t / metade);
            yield return null;
        }

        transform.localScale = _escalaOriginal;
        if (_spriteRenderer != null && !manterCorFinal)
        {
            _spriteRenderer.color = _corOriginal;
        }
        _rotinaFeedback = null;
    }
}

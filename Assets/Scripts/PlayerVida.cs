using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVida : MonoBehaviour
{
    public static PlayerVida Instance { get; private set; }

    public static event Action<int, int> OnVidaAlterada;
    public static event Action OnPlayerMorreu;

    [SerializeField] private int _vidaMaxima = 5;
    [SerializeField] private float _tempoInvencivel = 1.0f;

    private int _vidaAtual;
    private bool _invencivel;
    private SpriteRenderer _spriteRenderer;
    private Color _corOriginal;
    private Coroutine _rotinaInvencibilidade;

    public int VidaAtual => _vidaAtual;
    public int VidaMaxima => _vidaMaxima;
    public bool EstaMorto => _vidaAtual <= 0;
    public bool EstaVidaCheia => _vidaAtual >= _vidaMaxima;

    private void Awake()
    {
        Instance = this;
        _vidaMaxima = Mathf.Max(1, _vidaMaxima);
        _vidaAtual = _vidaMaxima;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _corOriginal = _spriteRenderer.color;
        }
    }

    private void Start()
    {
        OnVidaAlterada?.Invoke(_vidaAtual, _vidaMaxima);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void TomarDano(int quantidade)
    {
        if (quantidade <= 0 || _invencivel || EstaMorto) return;

        _vidaAtual = Mathf.Max(0, _vidaAtual - quantidade);
        OnVidaAlterada?.Invoke(_vidaAtual, _vidaMaxima);

        if (_vidaAtual <= 0)
        {
            if (_rotinaInvencibilidade != null)
            {
                StopCoroutine(_rotinaInvencibilidade);
                _rotinaInvencibilidade = null;
            }
            _invencivel = false;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 1f);
            }
            OnPlayerMorreu?.Invoke();
            return;
        }

        if (_rotinaInvencibilidade != null)
        {
            StopCoroutine(_rotinaInvencibilidade);
        }
        _rotinaInvencibilidade = StartCoroutine(RotinaPiscarInvencivel());
    }

    public void RecuperarVida(int quantidade)
    {
        if (quantidade <= 0 || EstaMorto || EstaVidaCheia) return;

        _vidaAtual = Mathf.Min(_vidaMaxima, _vidaAtual + quantidade);
        OnVidaAlterada?.Invoke(_vidaAtual, _vidaMaxima);
    }

    private IEnumerator RotinaPiscarInvencivel()
    {
        _invencivel = true;
        float tempoDecorrido = 0f;
        float intervaloPisca = 0.1f;
        bool vermelho = true;

        while (tempoDecorrido < _tempoInvencivel)
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = vermelho ? new Color(1f, 0.25f, 0.25f, 1f) : _corOriginal;
            }
            vermelho = !vermelho;
            yield return new WaitForSeconds(intervaloPisca);
            tempoDecorrido += intervaloPisca;
        }

        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _corOriginal;
        }
        _invencivel = false;
        _rotinaInvencibilidade = null;
    }
}

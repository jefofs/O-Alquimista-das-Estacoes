using System;
using UnityEngine;

public class InventarioAlquimista : MonoBehaviour
{
    public static InventarioAlquimista Instance { get; private set; }
    public static event Action OnInventarioAlterado;
    public static event Action OnFerramentaAlterada;

    [SerializeField] private int _frutas;
    [SerializeField] private int _cristais;
    [SerializeField] private int _raizes;
    [SerializeField] private int _madeiras;
    [SerializeField] private int _pedras;
    [SerializeField] private bool _temPicareta;

    public bool TemPicareta => _temPicareta;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void DesbloquearPicareta()
    {
        if (_temPicareta) return;

        _temPicareta = true;
        OnFerramentaAlterada?.Invoke();
        OnInventarioAlterado?.Invoke();
    }

    public bool TemRecursos(int madeira, int pedra)
    {
        return _madeiras >= madeira && _pedras >= pedra;
    }

    public bool TentarConsumir(TipoIngrediente tipo, int quantidade)
    {
        return ConsumirQuantidade(tipo, quantidade);
    }

    public bool ConsumirQuantidade(TipoIngrediente tipo, int quantidade)
    {
        if (quantidade <= 0) return true;

        int atual = ObterQuantidade(tipo);
        if (atual < quantidade) return false;

        DefinirQuantidade(tipo, atual - quantidade);
        OnInventarioAlterado?.Invoke();
        return true;
    }

    public void Adicionar(TipoIngrediente tipo, int quantidade = 1)
    {
        if (quantidade <= 0) return;

        DefinirQuantidade(tipo, ObterQuantidade(tipo) + quantidade);
        OnInventarioAlterado?.Invoke();
    }

    public int ObterQuantidade(TipoIngrediente tipo)
    {
        return tipo switch
        {
            TipoIngrediente.Fruta => _frutas,
            TipoIngrediente.Cristal => _cristais,
            TipoIngrediente.Raiz => _raizes,
            TipoIngrediente.Madeira => _madeiras,
            TipoIngrediente.Pedra => _pedras,
            _ => 0
        };
    }

    public int ConsumirTudo(TipoIngrediente tipo)
    {
        int atual = ObterQuantidade(tipo);
        if (atual <= 0) return 0;

        DefinirQuantidade(tipo, 0);
        OnInventarioAlterado?.Invoke();
        return atual;
    }

    public int ConsumirAte(TipoIngrediente tipo, int limiteMaximo)
    {
        if (limiteMaximo <= 0) return 0;

        int total = ConsumirTudo(tipo);
        if (total > limiteMaximo)
        {
            int excedente = total - limiteMaximo;
            Adicionar(tipo, excedente);
            return limiteMaximo;
        }

        return total;
    }

    private void DefinirQuantidade(TipoIngrediente tipo, int valor)
    {
        int v = Mathf.Max(0, valor);
        switch (tipo)
        {
            case TipoIngrediente.Fruta:
                _frutas = v;
                break;
            case TipoIngrediente.Cristal:
                _cristais = v;
                break;
            case TipoIngrediente.Raiz:
                _raizes = v;
                break;
            case TipoIngrediente.Madeira:
                _madeiras = v;
                break;
            case TipoIngrediente.Pedra:
                _pedras = v;
                break;
        }
    }
}

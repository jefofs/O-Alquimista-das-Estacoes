using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ArbustoFrutas : MonoBehaviour, IInteragivel
{
    [SerializeField] private GameObject _prefabFruta;
    [SerializeField] private Transform _indicadorFrutas;
    [SerializeField] private float _tempoRegeneracao = 8f;
    [SerializeField] private int _minFrutas = 1;
    [SerializeField] private int _maxFrutas = 2;

    private bool _comFrutas = true;
    private float _cronometroRegeneracao;

    public bool ComFrutas => _comFrutas;

    private void Awake()
    {
        if (_indicadorFrutas == null)
        {
            Transform filho = transform.Find("IndicadorFrutas");
            if (filho != null) _indicadorFrutas = filho;
        }
        AtualizarVisual();
    }

    private void Update()
    {
        if (!_comFrutas)
        {
            _cronometroRegeneracao += Time.deltaTime;
            if (_cronometroRegeneracao >= _tempoRegeneracao)
            {
                _comFrutas = true;
                _cronometroRegeneracao = 0f;
                AtualizarVisual();
            }
        }
    }

    public void Interagir(PlayerAcoes player)
    {
        if (!_comFrutas)
        {
            RecursoMineravel.EmitirAviso("Este arbusto ainda está regenerando frutas...");
            return;
        }

        _comFrutas = false;
        _cronometroRegeneracao = 0f;
        AtualizarVisual();

        int quantidade = Random.Range(_minFrutas, _maxFrutas + 1);
        if (_prefabFruta != null)
        {
            for (int i = 0; i < quantidade; i++)
            {
                Vector2 direcaoDrop = Random.insideUnitCircle.normalized;
                if (direcaoDrop.sqrMagnitude < 0.01f) direcaoDrop = Vector2.down;
                Vector3 posDrop = transform.position + (Vector3)(direcaoDrop * Random.Range(0.85f, 1.25f));
                Instantiate(_prefabFruta, posDrop, Quaternion.identity);
            }
        }
        else if (InventarioAlquimista.Instance != null)
        {
            InventarioAlquimista.Instance.Adicionar(TipoIngrediente.Fruta, quantidade);
        }
    }

    public string ObterPromptInteracao()
    {
        return _comFrutas ? "[E] Colher Arbusto" : "Arbusto regenerando frutas...";
    }

    private void AtualizarVisual()
    {
        if (_indicadorFrutas != null)
        {
            _indicadorFrutas.gameObject.SetActive(_comFrutas);
        }
    }
}

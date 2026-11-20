using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class IngredienteColetavel : MonoBehaviour
{
    [SerializeField] private TipoIngrediente _tipo = TipoIngrediente.Fruta;
    [SerializeField] private int _valor = 1;
    [SerializeField] private float _amplitudeFlutuacao = 0.12f;
    [SerializeField] private float _frequenciaFlutuacao = 3f;

    private Vector3 _posicaoBase;
    private float _faseAleatoria;
    private bool _coletado;

    public TipoIngrediente Tipo => _tipo;

    private void Start()
    {
        _posicaoBase = transform.position;
        _faseAleatoria = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        float deslocamentoY = Mathf.Sin((Time.time * _frequenciaFlutuacao) + _faseAleatoria) * _amplitudeFlutuacao;
        transform.position = new Vector3(_posicaoBase.x, _posicaoBase.y + deslocamentoY, _posicaoBase.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_coletado) return;

        bool ehPlayer = other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null;
        if (!ehPlayer) return;

        InventarioAlquimista inventario = InventarioAlquimista.Instance != null
            ? InventarioAlquimista.Instance
            : other.GetComponent<InventarioAlquimista>();

        if (inventario != null)
        {
            _coletado = true;
            inventario.Adicionar(_tipo, _valor);
            Destroy(gameObject);
        }
    }
}

using UnityEngine;

public class SpawnerIngredientes : MonoBehaviour
{
    [Header("Prefabs de Nós Mineráveis")]
    [SerializeField] private GameObject _prefabArvore;
    [SerializeField] private GameObject _prefabRocha;
    [SerializeField] private GameObject _prefabVeioCristal;

    [Header("Configuração de Respawn")]
    [SerializeField] private float _intervaloSpawn = 4f;
    [SerializeField] private int _minimoNaCena = 6;
    [SerializeField] private int _maximoNaCena = 8;
    [SerializeField] private Vector2 _limiteMin = new Vector2(-8.2f, -5.2f);
    [SerializeField] private Vector2 _limiteMax = new Vector2(8.2f, 5.2f);
    [SerializeField] private float _raioEspacoLivre = 1.35f;
    [SerializeField] private Transform _containerRecursos;

    private float _cronometro;

    private void Update()
    {
        _cronometro += Time.deltaTime;
        if (_cronometro >= _intervaloSpawn)
        {
            _cronometro = 0f;
            TentarRespawnarRecurso();
        }
    }

    private void TentarRespawnarRecurso()
    {
        RecursoMineravel[] ativos = FindObjectsByType<RecursoMineravel>();
        int totalAtivos = ativos != null ? ativos.Length : 0;

        if (totalAtivos >= _minimoNaCena || totalAtivos >= _maximoNaCena)
        {
            return;
        }

        GameObject prefabEscolhido = EscolherRecursoEmFalta(ativos);
        if (prefabEscolhido == null) return;

        if (TentarGerarPosicaoLivre(out Vector3 posicao))
        {
            GameObject novo = Instantiate(prefabEscolhido, posicao, Quaternion.identity);
            if (_containerRecursos != null)
            {
                novo.transform.SetParent(_containerRecursos, true);
            }
        }
    }

    private GameObject EscolherRecursoEmFalta(RecursoMineravel[] ativos)
    {
        int arvores = 0;
        int rochas = 0;
        int cristais = 0;

        if (ativos != null)
        {
            for (int i = 0; i < ativos.Length; i++)
            {
                string nome = ativos[i].NomeRecurso;
                if (nome == "Arvore") arvores++;
                else if (nome == "Rocha") rochas++;
                else if (nome == "Cristal") cristais++;
            }
        }

        if (arvores < 2 && _prefabArvore != null) return _prefabArvore;
        if (rochas < 2 && _prefabRocha != null) return _prefabRocha;
        if (cristais < 2 && _prefabVeioCristal != null) return _prefabVeioCristal;

        float sorteio = Random.value;
        if (sorteio < 0.4f && _prefabArvore != null) return _prefabArvore;
        if (sorteio < 0.75f && _prefabRocha != null) return _prefabRocha;
        return _prefabVeioCristal != null ? _prefabVeioCristal : _prefabArvore;
    }

    private bool TentarGerarPosicaoLivre(out Vector3 posicaoEscolhida)
    {
        for (int tentativa = 0; tentativa < 25; tentativa++)
        {
            float x = Random.Range(_limiteMin.x, _limiteMax.x);
            float y = Random.Range(_limiteMin.y, _limiteMax.y);
            Vector2 candidata = new Vector2(x, y);

            if (candidata.magnitude < 2.5f) continue;

            Collider2D ocupado = Physics2D.OverlapCircle(candidata, _raioEspacoLivre);
            if (ocupado == null)
            {
                posicaoEscolhida = new Vector3(candidata.x, candidata.y, 0f);
                return true;
            }
        }

        posicaoEscolhida = Vector3.zero;
        return false;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public enum EstadoJogo
{
    Jogando,
    Vitoria,
    Derrota
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Transição de Fase")]
    [SerializeField] private string _nomeProximaCena = "";

    [Header("Painéis de Fim de Jogo")]
    [SerializeField] private GameObject _painelVitoria;
    [SerializeField] private GameObject _painelDerrota;

    public EstadoJogo EstadoAtual { get; private set; } = EstadoJogo.Jogando;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Time.timeScale = 1f;
        EstadoAtual = EstadoJogo.Jogando;
    }

    private void OnEnable()
    {
        Caldeirao.OnReceitaCompleta += AoVencerFase;
        PlayerVida.OnPlayerMorreu += AoPerderFase;
    }

    private void OnDisable()
    {
        Caldeirao.OnReceitaCompleta -= AoVencerFase;
        PlayerVida.OnPlayerMorreu -= AoPerderFase;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        EstadoAtual = EstadoJogo.Jogando;

        if (_painelVitoria != null) _painelVitoria.SetActive(false);
        if (_painelDerrota != null) _painelDerrota.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void AoVencerFase()
    {
        if (EstadoAtual != EstadoJogo.Jogando) return;

        EstadoAtual = EstadoJogo.Vitoria;
        if (_painelVitoria != null)
        {
            _painelVitoria.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    private void AoPerderFase()
    {
        if (EstadoAtual != EstadoJogo.Jogando) return;

        EstadoAtual = EstadoJogo.Derrota;
        if (_painelDerrota != null)
        {
            _painelDerrota.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void ReiniciarFase()
    {
        Time.timeScale = 1f;
        Scene cenaAtual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(cenaAtual.name);
    }

    public void CarregarProximaFase()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrWhiteSpace(_nomeProximaCena) && Application.CanStreamedLevelBeLoaded(_nomeProximaCena))
        {
            SceneManager.LoadScene(_nomeProximaCena);
        }
        else
        {
            Debug.Log("Próxima ilha ainda não configurada no Build Profile. Reiniciando fase atual.");
            ReiniciarFase();
        }
    }

    public void VoltarAoMenu()
    {
        Time.timeScale = 1f;
        if (Application.CanStreamedLevelBeLoaded("Menu"))
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            ReiniciarFase();
        }
    }
}

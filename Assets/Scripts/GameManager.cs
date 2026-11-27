using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private string _nomeCenaMenu = "Menu";
    [SerializeField] private string _nomeCenaVitoriaFinal = "VitoriaFinal";

    [Header("Painéis de UI")]
    [SerializeField] private GameObject _painelVitoria;
    [SerializeField] private GameObject _painelDerrota;
    [SerializeField] private GameObject _painelPausa;

    private bool _estaPausado;

    public EstadoJogo EstadoAtual { get; private set; } = EstadoJogo.Jogando;
    public bool EstaPausado => _estaPausado;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Time.timeScale = 1f;
        _estaPausado = false;
        EstadoAtual = EstadoJogo.Jogando;
        ResolverReferenciasUI();
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
        _estaPausado = false;
        EstadoAtual = EstadoJogo.Jogando;
        ResolverReferenciasUI();

        if (_painelVitoria != null) _painelVitoria.SetActive(false);
        if (_painelDerrota != null) _painelDerrota.SetActive(false);
        if (_painelPausa != null) _painelPausa.SetActive(false);
    }

    private void Update()
    {
        if (EstadoAtual != EstadoJogo.Jogando) return;

        Keyboard teclado = Keyboard.current;
        if (teclado != null && teclado.escapeKey.wasPressedThisFrame)
        {
            AlternarPausa();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void ResolverReferenciasUI()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        if (_painelVitoria == null)
        {
            Transform t = canvas.transform.Find("PainelVitoria");
            if (t != null) _painelVitoria = t.gameObject;
        }
        if (_painelDerrota == null)
        {
            Transform t = canvas.transform.Find("PainelDerrota");
            if (t != null) _painelDerrota = t.gameObject;
        }
        if (_painelPausa == null)
        {
            Transform t = canvas.transform.Find("PainelPausa");
            if (t != null) _painelPausa = t.gameObject;
        }

        VincularBotaoSeNecessario(_painelVitoria, "BotaoJogarNovamente", ReiniciarFase);
        VincularBotaoSeNecessario(_painelVitoria, "BotaoProximaIlha", CarregarProximaFase);
        VincularBotaoSeNecessario(_painelVitoria, "BotaoMenuPrincipal", VoltarAoMenu);

        VincularBotaoSeNecessario(_painelDerrota, "BotaoTentarNovamente", ReiniciarFase);
        VincularBotaoSeNecessario(_painelDerrota, "BotaoMenuPrincipal", VoltarAoMenu);

        VincularBotaoSeNecessario(_painelPausa, "BotaoContinuar", ContinuarJogo);
        VincularBotaoSeNecessario(_painelPausa, "BotaoReiniciarFase", ReiniciarFase);
        VincularBotaoSeNecessario(_painelPausa, "BotaoMenuPrincipal", VoltarAoMenu);
    }

    private void VincularBotaoSeNecessario(GameObject painel, string nomeBotao, UnityAction acao)
    {
        if (painel == null) return;
        Transform t = painel.transform.Find(nomeBotao);
        if (t == null) return;
        Button btn = t.GetComponent<Button>();
        if (btn != null && btn.onClick.GetPersistentEventCount() == 0)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(acao);
        }
    }

    public void AlternarPausa()
    {
        if (EstadoAtual != EstadoJogo.Jogando) return;

        if (_estaPausado)
        {
            ContinuarJogo();
        }
        else
        {
            PausarJogo();
        }
    }

    public void PausarJogo()
    {
        if (EstadoAtual != EstadoJogo.Jogando) return;

        _estaPausado = true;
        if (_painelPausa != null) _painelPausa.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ContinuarJogo()
    {
        _estaPausado = false;
        if (_painelPausa != null) _painelPausa.SetActive(false);
        if (EstadoAtual == EstadoJogo.Jogando)
        {
            Time.timeScale = 1f;
        }
    }

    private void AoVencerFase()
    {
        if (EstadoAtual != EstadoJogo.Jogando) return;

        _estaPausado = false;
        EstadoAtual = EstadoJogo.Vitoria;
        if (_painelPausa != null) _painelPausa.SetActive(false);
        if (_painelVitoria != null) _painelVitoria.SetActive(true);
        Time.timeScale = 0f;
    }

    private void AoPerderFase()
    {
        if (EstadoAtual != EstadoJogo.Jogando) return;

        _estaPausado = false;
        EstadoAtual = EstadoJogo.Derrota;
        if (_painelPausa != null) _painelPausa.SetActive(false);
        if (_painelDerrota != null) _painelDerrota.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ReiniciarFase()
    {
        _estaPausado = false;
        Time.timeScale = 1f;
        Scene cenaAtual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(cenaAtual.name);
    }

    public void CarregarProximaFase()
    {
        _estaPausado = false;
        Time.timeScale = 1f;

        if (!string.IsNullOrWhiteSpace(_nomeProximaCena) && Application.CanStreamedLevelBeLoaded(_nomeProximaCena))
        {
            SceneManager.LoadScene(_nomeProximaCena);
            return;
        }

        if (!string.IsNullOrWhiteSpace(_nomeCenaVitoriaFinal) && Application.CanStreamedLevelBeLoaded(_nomeCenaVitoriaFinal))
        {
            SceneManager.LoadScene(_nomeCenaVitoriaFinal);
            return;
        }

        Debug.Log("Próxima ilha ou cena VitoriaFinal não encontrada no Build Profile. Reiniciando fase atual.");
        ReiniciarFase();
    }

    public void VoltarAoMenu()
    {
        _estaPausado = false;
        Time.timeScale = 1f;

        if (!string.IsNullOrWhiteSpace(_nomeCenaMenu) && Application.CanStreamedLevelBeLoaded(_nomeCenaMenu))
        {
            SceneManager.LoadScene(_nomeCenaMenu);
        }
        else
        {
            ReiniciarFase();
        }
    }
}

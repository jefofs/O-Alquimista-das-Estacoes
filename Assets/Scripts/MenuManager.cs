using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Cenas")]
    [SerializeField] private string _cenaPrimeiraIlha = "Ilha_01_Primavera";
    [SerializeField] private string _cenaMenu = "Menu";

    [Header("Painéis do Menu")]
    [SerializeField] private GameObject _painelControles;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        if (_painelControles != null)
        {
            _painelControles.SetActive(false);
        }
    }

    public void BtnStartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_cenaPrimeiraIlha);
    }

    public void BtnAbrirControles()
    {
        if (_painelControles != null)
        {
            _painelControles.SetActive(true);
        }
    }

    public void BtnFecharControles()
    {
        if (_painelControles != null)
        {
            _painelControles.SetActive(false);
        }
    }

    public void BtnVoltarMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_cenaMenu);
    }

    public void BtnQuit()
    {
        Debug.Log("Sair do jogo");
        Application.Quit();
    }
}

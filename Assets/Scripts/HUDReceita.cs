using UnityEngine;
using UnityEngine.UI;

public class HUDReceita : MonoBehaviour
{
    [Header("Painel Superior Esquerdo")]
    [SerializeField] private Text _textoMochila;
    [SerializeField] private Text _textoFerramenta;
    [SerializeField] private Text _textoCaldeirao;

    [Header("Aviso / Prompt Inferior")]
    [SerializeField] private GameObject _painelAviso;
    [SerializeField] private Text _textoAviso;
    [SerializeField] private float _duracaoAviso = 3f;

    private string _promptInteracaoAtual = string.Empty;
    private string _avisoTemporarioAtual = string.Empty;
    private float _tempoFimAviso;

    private void OnEnable()
    {
        InventarioAlquimista.OnInventarioAlterado += AtualizarHUD;
        InventarioAlquimista.OnFerramentaAlterada += AtualizarHUD;
        Caldeirao.OnReceitaAlterada += AtualizarHUD;
        Caldeirao.OnReceitaCompleta += AoCompletarReceita;
        RecursoMineravel.OnAvisoJogador += ExibirAvisoTemporario;
        PlayerAcoes.OnPromptInteracaoAlterado += AoAlterarPromptInteracao;
    }

    private void OnDisable()
    {
        InventarioAlquimista.OnInventarioAlterado -= AtualizarHUD;
        InventarioAlquimista.OnFerramentaAlterada -= AtualizarHUD;
        Caldeirao.OnReceitaAlterada -= AtualizarHUD;
        Caldeirao.OnReceitaCompleta -= AoCompletarReceita;
        RecursoMineravel.OnAvisoJogador -= ExibirAvisoTemporario;
        PlayerAcoes.OnPromptInteracaoAlterado -= AoAlterarPromptInteracao;
    }

    private void Start()
    {
        AtualizarHUD();
        AtualizarTextoAvisoInferior();
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(_avisoTemporarioAtual) && Time.time >= _tempoFimAviso)
        {
            _avisoTemporarioAtual = string.Empty;
            AtualizarTextoAvisoInferior();
        }
    }

    public void AtualizarHUD()
    {
        AtualizarTextoMochila();
        AtualizarTextoFerramenta();
        AtualizarTextoCaldeirao();
    }

    private void AtualizarTextoMochila()
    {
        if (_textoMochila == null) return;

        int madeira = 0;
        int pedra = 0;
        int frutas = 0;
        int cristais = 0;

        if (InventarioAlquimista.Instance != null)
        {
            madeira = InventarioAlquimista.Instance.ObterQuantidade(TipoIngrediente.Madeira);
            pedra = InventarioAlquimista.Instance.ObterQuantidade(TipoIngrediente.Pedra);
            frutas = InventarioAlquimista.Instance.ObterQuantidade(TipoIngrediente.Fruta);
            cristais = InventarioAlquimista.Instance.ObterQuantidade(TipoIngrediente.Cristal);
        }

        _textoMochila.text = $"Mochila -> Madeira: {madeira} | Pedra: {pedra} | Frutas: {frutas} | Cristais: {cristais}";
    }

    private void AtualizarTextoFerramenta()
    {
        if (_textoFerramenta == null) return;

        bool temPicareta = InventarioAlquimista.Instance != null && InventarioAlquimista.Instance.TemPicareta;
        _textoFerramenta.text = temPicareta ? "Ferramenta: Picareta" : "Ferramenta: Mãos";
        _textoFerramenta.color = temPicareta
            ? new Color(0.45f, 0.95f, 1f, 1f)
            : new Color(0.88f, 0.88f, 0.88f, 1f);
    }

    private void AtualizarTextoCaldeirao()
    {
        if (_textoCaldeirao == null) return;

        Caldeirao caldeirao = Caldeirao.Instance != null
            ? Caldeirao.Instance
            : FindAnyObjectByType<Caldeirao>();

        if (caldeirao == null)
        {
            _textoCaldeirao.text = "Caldeirão -> Frutas: 0/3 | Cristais: 0/2";
            return;
        }

        if (caldeirao.ReceitaConcluida)
        {
            _textoCaldeirao.text = "Receita Completa! (Fase Concluída)";
            return;
        }

        if (caldeirao.MetaRaizes > 0)
        {
            _textoCaldeirao.text =
                $"Caldeirão -> Frutas: {caldeirao.FrutasEntregues}/{caldeirao.MetaFrutas} | " +
                $"Cristais: {caldeirao.CristaisEntregues}/{caldeirao.MetaCristais} | " +
                $"Raízes: {caldeirao.RaizesEntregues}/{caldeirao.MetaRaizes}";
        }
        else
        {
            _textoCaldeirao.text =
                $"Caldeirão -> Frutas: {caldeirao.FrutasEntregues}/{caldeirao.MetaFrutas} | " +
                $"Cristais: {caldeirao.CristaisEntregues}/{caldeirao.MetaCristais}";
        }
    }

    private void AoCompletarReceita()
    {
        if (_textoCaldeirao != null)
        {
            _textoCaldeirao.text = "Receita Completa! (Fase Concluída)";
        }
        ExibirAvisoTemporario("RECEITA COMPLETA! Ilha da Primavera purificada!");
    }

    private void ExibirAvisoTemporario(string mensagem)
    {
        _avisoTemporarioAtual = mensagem;
        _tempoFimAviso = Time.time + _duracaoAviso;
        AtualizarTextoAvisoInferior();
    }

    private void AoAlterarPromptInteracao(string prompt)
    {
        _promptInteracaoAtual = prompt;
        AtualizarTextoAvisoInferior();
    }

    private void AtualizarTextoAvisoInferior()
    {
        if (_textoAviso == null) return;

        string mensagemAtiva = !string.IsNullOrEmpty(_avisoTemporarioAtual)
            ? _avisoTemporarioAtual
            : _promptInteracaoAtual;

        bool mostrar = !string.IsNullOrEmpty(mensagemAtiva);
        if (_painelAviso != null)
        {
            _painelAviso.SetActive(mostrar);
        }

        _textoAviso.text = mensagemAtiva;
        _textoAviso.color = !string.IsNullOrEmpty(_avisoTemporarioAtual)
            ? new Color(1f, 0.88f, 0.35f, 1f)
            : Color.white;
    }
}

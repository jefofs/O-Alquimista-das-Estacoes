using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BancadaCraft : MonoBehaviour, IInteragivel
{
    [SerializeField] private int _custoMadeira = 3;
    [SerializeField] private int _custoPedra = 2;

    public void Interagir(PlayerAcoes player)
    {
        InventarioAlquimista inventario = InventarioAlquimista.Instance;
        if (inventario == null) return;

        if (inventario.TemPicareta)
        {
            RecursoMineravel.EmitirAviso("Você já possui a Picareta!");
            return;
        }

        if (inventario.TemRecursos(_custoMadeira, _custoPedra))
        {
            inventario.ConsumirQuantidade(TipoIngrediente.Madeira, _custoMadeira);
            inventario.ConsumirQuantidade(TipoIngrediente.Pedra, _custoPedra);
            inventario.DesbloquearPicareta();
            RecursoMineravel.EmitirAviso("Picareta criada! Agora você pode minerar Cristais!");
        }
        else
        {
            RecursoMineravel.EmitirAviso($"Faltam recursos: precisa de {_custoMadeira} Madeiras e {_custoPedra} Pedras");
        }
    }

    public string ObterPromptInteracao()
    {
        if (InventarioAlquimista.Instance != null && InventarioAlquimista.Instance.TemPicareta)
        {
            return "[E] Bancada: Picareta já criada";
        }
        return $"[E] Bancada: Criar Picareta ({_custoMadeira} Madeiras, {_custoPedra} Pedras)";
    }
}

using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.ViewModels
{

    public enum PedidoStatus
    {
        [Display(Name = "Aguardando Preparo")]
        Recebido = 1,
        [Display(Name = "Preparo Iniciado")]
        Preparo = 2,
        [Display(Name = "Preparo Finalizado")]
        Pronto = 3,
        [Display(Name = "Saiu para Entrega")]
        SaiuParaEntrega = 4,
        [Display(Name = "Entregue")]
        Entregue = 5
    }
}

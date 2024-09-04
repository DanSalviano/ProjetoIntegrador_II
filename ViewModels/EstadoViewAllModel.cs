using System.ComponentModel;

namespace PizzaDelivery.ViewModels
{
    public class EstadoViewAllModel
    {
        [DisplayName("UF")]
        public string Id { get; set; }

        public string Estado { get; set; }

        public string Capital { get; set; }


        [DisplayName("Ativo")]
        public bool IsAtivo { get; set; }

        [DisplayName("Excluído")]
        public bool IsExcluido { get; set; }
    }
}

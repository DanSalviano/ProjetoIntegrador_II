using System.ComponentModel;

namespace PizzaDelivery.ViewModels
{
    public class CidadeViewAllModel
    {
        public string Id { get; set; }

        public string Cidade { get; set; }

        [DisplayName("UF")]
        public string EstadoId { get; set; }

        public string Estado { get; set; }

        [DisplayName("Ativo")]
        public bool IsAtivo { get; set; }

        [DisplayName("Excluído")]
        public bool IsExcluido { get; set; }
    }
}

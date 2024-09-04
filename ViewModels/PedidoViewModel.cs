using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.ViewModels
{
    public class PedidoViewModel
    {
        public string Id { get; set; }
        public IEnumerable<ShoppingCartItemsViewModel> ShoppingCartItems { get; set; }

        [DisplayName("Status")]
        public PedidoStatus Status { get; set; } = PedidoStatus.Recebido;

        [DisplayName("Endereço")]
        [Required(ErrorMessage = "Digite um CEP válido para obter o {0}")]
        public string Logradouro { get; set; }


        [Required(ErrorMessage = "Digite o {0}")]
        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }


        private string _cep;

        [Required(ErrorMessage = "Digite o {0}")]
        public string CEP
        {
            get => _cep ?? string.Empty;
            set
            {
                value = value.Replace(".", "").Replace("-", "");
                _cep = $"{value.Substring(0,2)}.{value.Substring(2, 3)}-{value.Substring(5,3)}";
            }
        }

        public string Referencia { get; set; }

        [Required(ErrorMessage = "Escolha a Forma de Pagamento")]
        [DisplayName("Forma de pagamento:")]
        public FormaPagamento FormaPagamento { get; set; }

        [Range(0.00, 100.00, ErrorMessage = "Digite um valor até R$ {2} para o {0}")]
        public double? Troco { get; set; }

        [StringLength(200)]
        [DisplayName("Observações:")]
        public string Observacao { get; set; }

        public string EnderecoCompleto
        {
            get
            {
                var cep = CEP.Replace(".", "").Replace("-", "");
                var cep_formatado = $"{cep.Substring(0, 2)}.{cep.Substring(2, 3)}-{cep.Substring(5, 3)}";
                return $"{Logradouro}, {Numero} {Complemento}, {Bairro}, {Cidade}, {Estado}, CEP {cep_formatado}";
            }
        }

        public bool IsAtivo { get; set; } = true;

    }
}

using Microsoft.CodeAnalysis.CSharp.Syntax;
using PizzaDelivery.Extensions;
using PizzaDelivery.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.ViewModels
{

    public class PedidoViewAllModel
    {
        public PedidoViewAllModel()
        {
            PedidoItens = new List<PedidoItemViewModel>();
            Roles = new List<string>();
        }

        public string Id { get; set; }
        public string UsuarioIdInclusao { get; set; }

        [DisplayName("Cliente")]
        public string Cliente { get; set; }

        public List<string> Roles { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Data Do Pedido")]
        public DateTime? DataDoPedido { get; set; }

        public List<PedidoItemViewModel> PedidoItens { get; set; }

        public decimal Total => PedidoItens.Sum(item => item.Subtotal);

        public PedidoStatus Status { get; set; }

        [DisplayName("Status")]
        public string GetStatus
        {
            get
            {
                return Status switch
                {
                    PedidoStatus.Recebido => "Aguardando Preparo",
                    PedidoStatus.Preparo => "Preparo Iniciado",
                    PedidoStatus.Pronto => "Preparo Finalizado",
                    PedidoStatus.SaiuParaEntrega => "Saiu Para Entrega",
                    PedidoStatus.Entregue => "Entregue",
                    _ => throw new ArgumentOutOfRangeException(nameof(Status), $"Status Inválido: {Status}")
                };
            }
        }

        public string Logradouro { private get; set; }

        public string Numero { private get; set; }

        public string Complemento { get; set; }

        public string Bairro { private get; set; }

        public string Cidade { private get; set; }

        public string Estado { private get; set; }

        public string CEP { private get; set; }

        public string Referencia { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        [DisplayName("Pagamento")]
        public string GetFormaPagamento
        {
            get
            {
                return FormaPagamento switch
                {
                    FormaPagamento.Pix => "Pix",
                    FormaPagamento.Card => "Cartão",
                    FormaPagamento.Cash => "Dinheiro",
                    _ => throw new ArgumentOutOfRangeException(nameof(FormaPagamento), $"Forma de Pagamento Inválida: {FormaPagamento}")
                };
            }
        }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public double? Troco { get; set; }


        [DisplayName("Observações:")]
        public string Observacao { get; set; }


        [DataType(DataType.PhoneNumber)]
        public string Telefone { get; set; }



        public DateTime? DataInicioPreparo { get; set; }

        [MaxLength(36)]
        public string UsuarioIdInicioPreparo { get; set; }

        [MaxLength(100)]
        public string UsuarioNomeInicioPreparo { get; set; }


        public DateTime? DataFimPreparo { get; set; }

        [MaxLength(36)]
        public string UsuarioIdFimPreparo { get; set; }

        [MaxLength(100)]
        public string UsuarioNomeFimPreparo { get; set; }



        public DateTime? DataInicioEntrega { get; set; }

        [MaxLength(36)]
        public string UsuarioIdInicioEntrega { get; set; }

        [MaxLength(100)]
        public string UsuarioNomeInicioEntrega { get; set; }


        public DateTime? DataFimEntrega { get; set; }

        [MaxLength(36)]
        public string UsuarioIdFimEntrega { get; set; }

        [MaxLength(100)]
        public string UsuarioNomeFimEntrega { get; set; }

        private string _complemento
        {
            get
            {
                return Complemento == null ? "" : $", {Complemento}"; //
            }
        }



        // Gets

        [DisplayName("Endereço de Entrega")]
        public string EnderecoCompleto
        {
            get
            {
                var cep = CEP.Replace(".", "").Replace("-", "");
                var cep_formatado = $"{cep.Substring(0, 2)}.{cep.Substring(2, 3)}-{cep.Substring(5, 3)}";
                return $"{Logradouro}, {Numero}{_complemento}, {Bairro}, {Cidade}, {Estado}, CEP {cep_formatado}";
            }
        }

        [DisplayName("Código")]
        public string GetCodigo => Id.Substring(Id.LastIndexOf("-") + 1);


    }
}

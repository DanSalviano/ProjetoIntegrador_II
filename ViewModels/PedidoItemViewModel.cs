using PizzaDelivery.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.ViewModels
{
    public class PedidoItemViewModel
    {
        public ProdutoModel Produto { get; set; }

        [DisplayName("Preço")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Preco { get; set; }

        [DisplayName("Conteúdo")]
        public string GetConteudo
        {
            get
            {
                if (Produto?.Conteudo > 0 && !string.IsNullOrEmpty(Produto?.Medida))
                {
                    return Produto.Medida switch
                    {
                        "unid" => (Produto.Conteudo > 1 ? $"{Produto.Conteudo} unidades" : "1 unidade"),
                        "L" => $"{Produto.Conteudo} {(Produto.Conteudo > 1 ? "litros" : "litro")}",
                        "ml" => $"{Produto.Conteudo}ml",
                        "Kg" => $"{Produto.Conteudo}Kg",
                        "g" => $"{Produto.Conteudo}g",
                        "mg" => $"{Produto.Conteudo}mg",
                        "Dz" => $"{Produto.Conteudo} {(Produto.Conteudo > 1 ? "dúzias" : "dúzia")}",
                        "Cento" => $"{Produto.Conteudo} {(Produto.Conteudo > 1 ? "centos" : "cento")}",
                        _ => throw new ArgumentOutOfRangeException(nameof(Produto.Medida), $"Medida inválida: {Produto.Medida}")
                    };
                }
                return string.Empty;
            }

        }

        public int Quantidade { get; set; }

        [DisplayName("Preço")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Subtotal => Preco * Quantidade;
    }
}

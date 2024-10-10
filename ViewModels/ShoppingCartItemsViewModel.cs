using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.ViewModels
{
    public class ShoppingCartItemsViewModel
    {
        public string Id { get; set; }
        public string ProdutoId { get; set; }

        [DisplayName("Nome do Produto")]
        public string Produto { get; set; }

        public int Conteudo { get; set; }

        public string Medida { get; set; } // l, unid, m, cm, kg, g

        public string Ingredientes { get; set; }

        public string NomeArquivoImagem { get; set; }

        [DisplayName("Imagem")]
        public string CaminhoImagem
        {
            get
            {
                if (!string.IsNullOrEmpty(NomeArquivoImagem))
                {
                    return Path.Combine($"\\img\\products\\", NomeArquivoImagem);
                }
                else
                {
                    return Path.Combine($"\\img\\assets\\sem_imagem.jpg");

                }
            }
        }

        [DisplayName("Conteúdo")]
        public string GetConteudo
        {
            get
            {
                if (Conteudo > 0 && !string.IsNullOrEmpty(Medida))
                {
                    return Medida switch
                    {
                        "unid" => (Conteudo > 1 ? $"{Conteudo} unidades" : "unidade"),
                        "L" => $"{Conteudo} {(Conteudo > 1 ? "litros" : "litro")}",
                        "ml" => $"{Conteudo}ml",
                        "Kg" => $"{Conteudo}Kg",
                        "g" => $"{Conteudo}g",
                        "mg" => $"{Conteudo}mg",
                        "Dz" => $"{Conteudo} {(Conteudo > 1 ? "dúzias" : "dúzia")}",
                        "Cento" => $"{Conteudo} {(Conteudo > 1 ? "centos" : "cento")}",
                        _ => throw new ArgumentOutOfRangeException(nameof(Medida), $"Medida inválida: {Medida}")
                    };
                }
                return string.Empty;
            }

        }

        [DisplayName("Preço")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Preco { get; set; }

        public int Quantidade { get; set; }

        [DisplayName("Subtotal")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Subtotal { get => Quantidade * Preco; }
    }
}

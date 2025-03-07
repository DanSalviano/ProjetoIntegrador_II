using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.ViewModels
{
    public class ProdutoAddOrEditViewModel
    {
        public string Id { get; set; }

        [DisplayName("Nome do Produto")]
        [Required(ErrorMessage = "Informe o {0}.")]
        [StringLength(100, ErrorMessage = "O {0} excedeu os {1} caracteres.")]
        public string Produto { get; set; }

        [Required(ErrorMessage = "Informe a {0}.")]
        [StringLength(200, ErrorMessage = "A {0} excedeu os {1} caracteres.")]
        public string Descricao { get; set; }

        [DisplayName("Conteúdo")]
        [Required(ErrorMessage = "Informe a {0}.")]
        [Range(0.01, 999.99, ErrorMessage = "Informe valores entre {1} e {2} para o {0}.")]
        public float Conteudo { get; set; }

        [Required(ErrorMessage = "Informe a {0}.")]
        public string Medida { get; set; } // l, unid, m, cm, kg, g

        [DisplayName("Ingredientes")]
        [Required(ErrorMessage = "Informe os {0}.")]
        [StringLength(200, ErrorMessage = "Os {0} excederam os {1} caracteres.")]
        public string Ingredientes { get; set; }

        [DisplayName("Categoria")]
        [Required(ErrorMessage = "Escolha a {0}.")]
        public string CategoriaId { get; set; }

        public List<SelectListItem> Categorias { get; set; }

        [DisplayName("Preço")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [Range(0.01, 9999.99, ErrorMessage = "Informe valores entre {1} e {2} para o {0}.")]
        public decimal Preco { get; set; }

        [Display(Name = "Arquivo da Imagem")]
        public IFormFile ArquivoImagem { get; set; }

        public string NomeArquivoImagem { get; set; }

        public string CaminhoImagem
        {
            get
            {
                if (!string.IsNullOrEmpty(NomeArquivoImagem))
                {
                    return Path.Combine($"\\img\\products\\", NomeArquivoImagem);
                }
                return Path.Combine($"\\img\\assets\\sem_imagem.webp");
            }
        }

        //*********[ Propriedades para Auditoria ]*********

        [DisplayName("Inclusão")]
        public string UsuarioIdInclusao { get; set; }

        //[DataType(DataType.Date)]
        [DisplayName("Data da Inclusão")]
        public DateTime? DataInclusao { get; set; }

        [DisplayName("Última Alteração")]
        public string UsuarioIdAlteracao { get; set; }

        //[DataType(DataType.Date)]  //Essa anotação mostra somente a data sem horário
        [DisplayName("Data da Alteração")]
        public DateTime? DataAlteracao { get; set; }



        [DisplayName("Ativo")]
        public bool IsAtivo { get; set; } = true;

        [DisplayName("Excluído")]
        public bool IsExcluido { get; set; }

        [DisplayName("Exclusão")]
        public string UsuarioIdExclusao { get; set; }

        //[DataType(DataType.Date)]
        [DisplayName("Data da Exclusão")]
        public DateTime? DataExclusao { get; set; }
    }
}

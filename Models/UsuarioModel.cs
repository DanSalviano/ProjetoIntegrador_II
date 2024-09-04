using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PizzaDelivery.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Models
{
    public class UsuarioModel : IdentityUser, IAuditable, ISoftDeletable
    {
        [Required]
        [MaxLength(36)]
        public string CidadeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string NomeCompleto { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        private string _cpf;
        [Required]
        [MaxLength(14)]
        public string CPF
        {
            get => _cpf ?? string.Empty; 
            set
            {
                _cpf = value.Replace(".", "").Replace("-", "");
            }
        }

        [Required]
        public bool IsAlteraSenhaLogin { get; set; }



        //*********[ Propriedades para Auditoria ]*********

        [Required]
        [MaxLength(36)]
        public string UsuarioIdInclusao { get; set; }

        [ReadOnly(true)]
        [DataType(DataType.Date)]
        public DateTime? DataInclusao { get; set; }

        [MaxLength(36)]
        public string UsuarioIdAlteracao { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataAlteracao { get; set; }



        [Required]
        public bool IsAtivo { get; set; }

        [Required]
        public bool IsExcluido { get; set; }

        [MaxLength(36)]
        public string UsuarioIdExclusao { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataExclusao { get; set; }
    }
}

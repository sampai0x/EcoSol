using System.ComponentModel.DataAnnotations;

namespace ECOSOL.API.DTOs.Fornecedores
{
    public class FornecedorCreateDto
    {
        [Required]
        public string Nome { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Senha { get; set; } = null!;

        [Required]
        public string Telefone { get; set; } = null!;

        [Required]
        public string Endereco { get; set; } = null!;

        [Required]
        public string Cidade { get; set; } = null!;

        [Required]
        public string Estado { get; set; } = null!;

        [Required]
        public string CpfCnpj { get; set; } = null!;
    }
}

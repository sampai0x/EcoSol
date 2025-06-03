using System.ComponentModel.DataAnnotations;

namespace ECOSOL.API.DTOs.Clientes
{
    public class ClienteCreateDto
    {
        [Required]
        public string Nome { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Senha { get; set; } = null!;

        [Required]
        public string EnderecoCompleto { get; set; } = null!;

        [Required]
        public string CpfCnpj { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations;

namespace ECOSOL.API.DTOs.Admin
{
    public class EnderecoRejeicaoDto
    {
        [Required]
        public string Motivo { get; set; } = null!;
    }
}

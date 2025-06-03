using ECOSOL.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECOSOL.API.DTOs.Admin
{
    public class ContratoStatusUpdateDto
    {
        [Required]
        public StatusContrato NovoStatus { get; set; }
    }
}

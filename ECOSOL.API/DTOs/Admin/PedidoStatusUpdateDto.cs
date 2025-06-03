using ECOSOL.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECOSOL.API.DTOs.Admin
{
    public class PedidoStatusUpdateDto
    {
        [Required]
        public StatusContrato NovoStatus { get; set; }
    }
}

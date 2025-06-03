using System.ComponentModel.DataAnnotations;

namespace ECOSOL.API.DTOs.Contratos
{
    public class ContratoCreateDto
    {
        [Required]
        public int ClienteId { get; set; }

        [Required]
        public decimal QuantidadeEnergia { get; set; }

        [Required]
        public decimal ValorContrato { get; set; }
    }
}

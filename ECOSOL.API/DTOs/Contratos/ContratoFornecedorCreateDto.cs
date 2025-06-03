using System.ComponentModel.DataAnnotations;

namespace ECOSOL.API.DTOs.Contratos
{
    public class ContratoFornecedorCreateDto
    {
        [Required]
        public decimal QuantidadeEnergia { get; set; }

        [Required]
        public decimal ValorContrato { get; set; }
    }
}

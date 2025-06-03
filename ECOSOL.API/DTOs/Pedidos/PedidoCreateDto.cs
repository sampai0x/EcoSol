using System.ComponentModel.DataAnnotations;

namespace ECOSOL.API.DTOs.Pedidos
{
    public class PedidoCreateDto
    {
        [Required]
        public decimal QuantidadeEnergia { get; set; }

        [Required]
        public decimal ValorContrato { get; set; }

        [Required]
        public string EnderecoEntrega { get; set; } = null!;
    }
}

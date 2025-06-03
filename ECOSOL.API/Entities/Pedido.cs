using ECOSOL.API.Enums;

namespace ECOSOL.API.Entities
{
    public class Pedido
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        public int EcoSolId { get; set; }            
        public EcoSol EcoSol { get; set; } = null!;

        public decimal QuantidadeEnergia { get; set; }
        public decimal ValorContrato { get; set; }
        public DateTime DataContrato { get; set; }

        public string EnderecoEntrega { get; set; } = null!;
        public StatusContrato Status { get; set; }
    }
}

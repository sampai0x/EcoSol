using ECOSOL.API.Enums;

namespace ECOSOL.API.Entities
{
    public class Contrato
    {
        public int Id { get; set; }

        public int FornecedorId { get; set; }
        public Fornecedor Fornecedor { get; set; } = null!;

        public int EcoSolId { get; set; }            
        public EcoSol EcoSol { get; set; } = null!;

        public decimal QuantidadeEnergia { get; set; }
        public decimal ValorContrato { get; set; }
        public DateTime DataContrato { get; set; }

        public StatusContrato Status { get; set; }
    }
}

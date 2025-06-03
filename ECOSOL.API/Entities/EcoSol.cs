namespace ECOSOL.API.Entities
{
    public class EcoSol
    {
        public int Id { get; set; }
        public decimal BancoEnergia { get; set; }
        public decimal Saldo { get; set; }

        public List<Cliente> Clientes { get; set; } = new();
        public List<Fornecedor> Fornecedores { get; set; } = new();
        public List<Contrato> Contratos { get; set; } = new();
        public List<Pedido> Pedidos { get; set; } = new();
    }
}

namespace ECOSOL.API.DTOs.Home
{
    public class HomeStatsDto
    {
        public decimal Saldo { get; set; }
        public decimal BancoEnergia { get; set; }

        public int TotalClientes { get; set; }
        public int TotalFornecedores { get; set; }

        public decimal EnergiaTransacionada { get; set; }
        public decimal ValorMovimentado { get; set; }
    }
}

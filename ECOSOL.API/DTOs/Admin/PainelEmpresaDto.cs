namespace ECOSOL.API.DTOs.Admin
{
    public class PainelEmpresaDto
    {
        public int TotalClientes { get; set; }
        public int TotalFornecedores { get; set; }

        public int TotalContratos { get; set; }
        public int TotalPedidos { get; set; }

        public decimal EnergiaContratada { get; set; }
        public decimal EnergiaSolicitada { get; set; }

        public decimal Saldo { get; set; }
        public decimal BancoEnergia { get; set; }
    }
}

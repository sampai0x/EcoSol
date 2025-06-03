namespace ECOSOL.API.DTOs.Dashboard
{
    public class ClienteDashboardDto
    {
        public int TotalPedidos { get; set; }
        public decimal TotalEnergiaConsumida { get; set; }
        public decimal TotalGasto { get; set; }
        public string StatusComprovante { get; set; } = null!;
        public string? ObservacaoRejeicao { get; set; }
    }
}

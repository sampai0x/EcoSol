namespace ECOSOL.API.DTOs.Admin
{
    public class OfertaEnergiaDto
    {
        public string Tipo { get; set; } = null!; // "Contrato" ou "Pedido"
        public int Id { get; set; }

        public string NomeUsuario { get; set; } = null!;
        public decimal QuantidadeEnergia { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
    }
}

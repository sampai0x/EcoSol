namespace ECOSOL.API.DTOs.Admin
{
    public class ClienteAdminDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string CpfCnpj { get; set; } = null!;
        public string EnderecoCompleto { get; set; } = null!;

        public string StatusAprovacaoComprovante { get; set; } = null!;
        public DateTime? DataEnvioComprovante { get; set; }           
        public DateTime? DataAprovacaoComprovante { get; set; }       
    }
}

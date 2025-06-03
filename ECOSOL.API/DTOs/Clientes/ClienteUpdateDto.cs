namespace ECOSOL.API.DTOs.Clientes
{
    public class ClienteUpdateDto
    {
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
        public string EnderecoCompleto { get; set; } = null!;
        public string CpfCnpj { get; set; } = null!;
    }
}

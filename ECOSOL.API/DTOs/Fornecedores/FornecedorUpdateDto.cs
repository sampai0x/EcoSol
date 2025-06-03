namespace ECOSOL.API.DTOs.Fornecedores
{
    public class FornecedorUpdateDto
    {
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Endereco { get; set; } = null!;
        public string Cidade { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string CpfCnpj { get; set; } = null!;
    }
}

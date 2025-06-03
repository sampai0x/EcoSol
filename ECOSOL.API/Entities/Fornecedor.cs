namespace ECOSOL.API.Entities
{
    public class Fornecedor
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Endereco { get; set; } = null!;
        public string Cidade { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string CpfCnpj { get; set; } = null!;

        public int EcoSolId { get; set; }
        public EcoSol EcoSol { get; set; } = null!;
    }
}

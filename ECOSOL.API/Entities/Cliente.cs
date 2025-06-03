using ECOSOL.API.Enums;

namespace ECOSOL.API.Entities
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
        public string EnderecoCompleto { get; set; } = null!;
        public string CpfCnpj { get; set; } = null!;

        public string? ComprovanteUrl { get; set; }
        public StatusAprovacao StatusAprovacao { get; set; } = StatusAprovacao.NaoEnviado;
        public DateTime? DataEnvioComprovante { get; set; }
        public DateTime? DataAprovacaoComprovante { get; set; }
        public string? ObservacaoRejeicao { get; set; }


        public int EcoSolId { get; set; }
        public EcoSol EcoSol { get; set; } = null!;
    }
}

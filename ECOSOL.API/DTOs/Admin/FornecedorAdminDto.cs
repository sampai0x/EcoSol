﻿namespace ECOSOL.API.DTOs.Admin
{
    public class FornecedorAdminDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string CpfCnpj { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Endereco { get; set; } = null!;
    }
}

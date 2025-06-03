using Bogus;
using ECOSOL.API.Entities;
using ECOSOL.API.Enums;

namespace ECOSOL.API.Data
{
    public class DbInitializer
    {
        public static void Seed(EcosolDbContext context)
        {
            if (context.Clientes.Any() || context.Fornecedores.Any() || context.EcoSols.Any())
                return;

            var ecosol = new EcoSol
            {
                BancoEnergia = 100_000,
                Saldo = 500_000
            };

            context.EcoSols.Add(ecosol);
            context.SaveChanges();

            var clienteFaker = new Faker<Cliente>()
                .RuleFor(c => c.Nome, f => f.Name.FullName())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.Senha, f => f.Internet.Password())
                .RuleFor(c => c.CpfCnpj, f => f.Random.Replace("###########"))
                .RuleFor(c => c.EcoSolId, ecosol.Id)
                .RuleFor(c => c.EnderecoCompleto, f =>
                    $"{f.Address.StreetAddress()}, {f.Address.City()}, {f.Address.StateAbbr()}, CEP: {f.Address.ZipCode()}"
                );

            var fornecedorFaker = new Faker<Fornecedor>()
                .RuleFor(f => f.Nome, f => f.Company.CompanyName())
                .RuleFor(f => f.Email, f => f.Internet.Email())
                .RuleFor(c => c.Senha, f => f.Internet.Password())
                .RuleFor(f => f.Telefone, f => f.Phone.PhoneNumber("##9########"))
                .RuleFor(f => f.Endereco, f => f.Address.StreetAddress())
                .RuleFor(f => f.Cidade, f => f.Address.City())
                .RuleFor(f => f.Estado, f => f.Address.StateAbbr())
                .RuleFor(f => f.CpfCnpj, f => f.Random.Replace("###############"))
                .RuleFor(f => f.EcoSolId, ecosol.Id);

            var clientes = clienteFaker.Generate(20);
            var fornecedores = fornecedorFaker.Generate(10);

            context.Clientes.AddRange(clientes);
            context.Fornecedores.AddRange(fornecedores);
            context.SaveChanges();

            var contratoFaker = new Faker<Contrato>()
                // .RuleFor(c => c.ClienteId, f => f.PickRandom(clientes).Id) // REMOVIDA ESTA LINHA
                .RuleFor(c => c.FornecedorId, f => f.PickRandom(fornecedores).Id)
                .RuleFor(c => c.EcoSolId, ecosol.Id)
                .RuleFor(c => c.QuantidadeEnergia, f => f.Random.Decimal(100, 1000))
                .RuleFor(c => c.ValorContrato, f => f.Random.Decimal(5000, 20000)) // Valor que ECOSOL paga
                .RuleFor(c => c.DataContrato, f => f.Date.Past(1).ToUniversalTime())
                .RuleFor(c => c.Status, f => f.PickRandom<StatusContrato>());

            var pedidoFaker = new Faker<Pedido>()
                .RuleFor(p => p.ClienteId, f => f.PickRandom(clientes).Id)
                .RuleFor(p => p.EcoSolId, ecosol.Id)
                .RuleFor(p => p.QuantidadeEnergia, f => f.Random.Decimal(50, 500))
                .RuleFor(p => p.ValorContrato, f => f.Random.Decimal(3000, 15000))
                .RuleFor(p => p.DataContrato, f => f.Date.Past(1).ToUniversalTime())
                .RuleFor(p => p.EnderecoEntrega, f => f.Address.FullAddress())
                .RuleFor(p => p.Status, f => f.PickRandom<StatusContrato>());

            var contratos = contratoFaker.Generate(15);
            var pedidos = pedidoFaker.Generate(30);

            context.Contratos.AddRange(contratos);
            context.Pedidos.AddRange(pedidos);
            context.SaveChanges();
        }
    }
}

using ECOSOL.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECOSOL.API.Data
{
    public class EcosolDbContext : DbContext
    {
        public EcosolDbContext(DbContextOptions<EcosolDbContext> options) : base(options) { }

        public DbSet<EcoSol> EcoSols { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contrato>()
                   .Property(c => c.Status)
                   .HasConversion<string>();

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Status)
                .HasConversion<string>();
        }
    }
}

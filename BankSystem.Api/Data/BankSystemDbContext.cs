using BankSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Api.Data
{
    public class BankSystemDbContext : DbContext
    {
        // Construtor necessário para a Injeção de Dependência
        public BankSystemDbContext(DbContextOptions<BankSystemDbContext> options) : base(options)
        {
        }

        public DbSet<Conta> Contas { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Nome)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(c => c.Cpf)
                    .HasMaxLength(11)
                    .IsRequired();
                entity.HasIndex(c => c.Cpf)
                    .IsUnique();
            });

            modelBuilder.Entity<Conta>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Id)
                    .HasDefaultValueSql("NEWID()");

                entity.HasIndex(c => c.Numero)
                    .IsUnique();

                entity.Property(c => c.Titular)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(c => c.Tipo)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(c => c.Saldo)
                    .HasColumnType("decimal(18, 2)");

                entity.Property(c => c.DataCriacao)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(c => c.Cliente)
                    .WithMany(cl => cl.Contas)
                    .HasForeignKey(c => c.ClienteId)
                    .IsRequired();

                entity.HasOne(c => c.Cliente)
                    .WithMany(cl => cl.Contas)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
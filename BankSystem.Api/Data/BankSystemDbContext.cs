using BankSystem.Api.Models;
using BankSystem.Api.Models.Interfaces;
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

        public DbSet<Transacao> Transacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Cliente>().HasQueryFilter(x => x.Ativo);
            modelBuilder.Entity<Conta>().HasQueryFilter(x => x.Ativo);

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Nome).HasMaxLength(100).IsRequired();
                entity.Property(c => c.Cpf).HasMaxLength(11).IsRequired();
                entity.HasIndex(c => c.Cpf).IsUnique();
                entity.Property(c => c.Celular).HasMaxLength(20).IsRequired();
                entity.Property(c => c.RendaMensal).HasColumnType("decimal(18, 2)");
                entity.Property(c => c.Ativo).HasDefaultValue(true);

                entity.OwnsOne(c => c.Endereco, endereco =>
                {
                    endereco.Property(e => e.Logradouro).HasColumnName("Endereco_Logradouro").HasMaxLength(150).IsRequired();
                    endereco.Property(e => e.Numero).HasColumnName("Endereco_Numero").HasMaxLength(20).IsRequired();
                    endereco.Property(e => e.Cep).HasColumnName("Endereco_Cep").HasMaxLength(10).IsRequired();
                    endereco.Property(e => e.Bairro).HasColumnName("Endereco_Bairro").HasMaxLength(100);
                    endereco.Property(e => e.Cidade).HasColumnName("Endereco_Cidade").HasMaxLength(100).IsRequired();
                    endereco.Property(e => e.Estado).HasColumnName("Endereco_UF").HasMaxLength(2).IsRequired();
                    endereco.Property(e => e.Complemento).HasColumnName("Endereco_Complemento").HasMaxLength(100);
                });
            });

            modelBuilder.Entity<Conta>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<ContaCorrente>("Corrente")
            .HasValue<ContaPoupanca>("Poupanca");

            modelBuilder.Entity<Conta>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Id)
                    .HasDefaultValueSql("NEWID()");

                entity.HasIndex(c => c.Numero)
                    .IsUnique();

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
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Transacao>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Valor).HasColumnType("decimal(18, 2)");

                entity.HasOne(t => t.ContaOrigem)
                    .WithMany()
                    .HasForeignKey(t => t.ContaOrigemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.ContaDestino)
                    .WithMany()
                    .HasForeignKey(t => t.ContaDestinoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted && e.Entity is ISoftDelete);

            foreach (var entry in entities)
            {
                entry.State = EntityState.Modified;

                var entity = (ISoftDelete)entry.Entity;
                entity.Excluir();
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
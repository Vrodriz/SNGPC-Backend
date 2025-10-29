using Microsoft.EntityFrameworkCore;
using SNGPC_B.Api.Models;

namespace SNGPC_B.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<Farmaceutico> Farmaceuticos { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Farmaceutico>(entity =>
            {
                entity.HasKey(f => f.Id);
                
                entity.Property(f => f.Nome)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(f => f.CRF)
                    .IsRequired()
                    .HasMaxLength(10);
                
                entity.Property(f => f.CRFUF)
                    .IsRequired()
                    .HasMaxLength(2);
                
                entity.Property(f => f.CPF)
                    .IsRequired()
                    .HasMaxLength(11);
                
                entity.Property(f => f.LoginANVISA)
                    .HasMaxLength(60);
                
                entity.Property(f => f.SenhaANVISA)
                    .HasMaxLength(20);
            });
        }
    }
}
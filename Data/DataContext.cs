using Microsoft.EntityFrameworkCore;
using ApiRestNet.Models;

namespace ApiRestNet.Data
{
       public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Partida> Partidas { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
   

    modelBuilder.Entity<Partida>()
        .HasKey(p => p.IdNombre);   // Definir clave primaria de Partida

}

    }
}
using Microsoft.EntityFrameworkCore;

namespace SalasInmobiliaria.Models
{
    public class DataContext : DbContext   //DbSet para la aplicacion movil
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<Propietario> Propietario { get; set; }
        public DbSet<Inquilino> Inquilino { get; set; }
        public DbSet<Inmueble> Inmueble { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Pago> Pago { get; set; }
        public DbSet<Contrato> Contrato { get; set; }

    }
}

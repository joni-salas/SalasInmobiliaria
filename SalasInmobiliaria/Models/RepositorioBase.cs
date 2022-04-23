namespace SalasInmobiliaria.Models
{
    public abstract class RepositorioBase
    {
        protected readonly IConfiguration configuration;
        protected readonly string connectionString;
        public RepositorioBase(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration["ConnectionStrings:DefaultConnection"];
            //trae el string de coneccion que esta en appsettings -> ConnectionStrings -> DefaultConnection
        }
    }
}

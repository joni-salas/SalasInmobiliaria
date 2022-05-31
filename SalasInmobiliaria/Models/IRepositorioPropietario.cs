namespace SalasInmobiliaria.Models
{
    public interface IRepositorioPropietario : IRepositorio<Propietario>
    {
        IList<Propietario> ObtenerActivosInactivos();
    }
}

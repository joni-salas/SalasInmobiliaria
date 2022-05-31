namespace SalasInmobiliaria.Models
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        IList<Inmueble> ObtenerActivosInactivosAlquilados();
        IList<Inmueble> InmueblesNuncaAlquilados();
        IList<Inmueble> BuscarPorPropietario(int id);

        //IList<Inmueble> InmueblesNuncaAlquilados(string desde, string hasta, int id); // hacer este consulta a la BD

    }
}

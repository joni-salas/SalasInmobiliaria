namespace SalasInmobiliaria.Models
{
    public interface IRepositorioContrato : IRepositorio<Contrato>
    {
        
        IList<Contrato> BuscarPorInquilino(int id);
        IList<Contrato> BuscarPorInmueble(int id);
        //IList<Contrato> obtenerXFecha(string desde, string hasta);
        int CancelarContrato(Contrato contrato);
    }
}

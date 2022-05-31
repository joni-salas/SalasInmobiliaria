namespace SalasInmobiliaria.Models
{
    public interface IRepositorioPago : IRepositorio<Pago>
    {
            IList<Pago> BuscarPorContrato(int id);
    }
}

namespace SalasInmobiliaria.Models
{
    public interface IRepositorio<T>
    {
        int Alta(T i);
        int Baja(T t);
        int Modificacion(T e);
        T ObtenerPorId(int id);
        IList<T> ObtenerTodos();
    }
}

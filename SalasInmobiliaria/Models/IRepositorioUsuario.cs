namespace SalasInmobiliaria.Models
{
    public interface IRepositorioUsuario : IRepositorio<Usuario>
    {
        int CambiarContraseña(Usuario usuario);
        int EditarPerfil(Usuario usuario);
        Usuario ObtenerPorEmail(string email);
        //int Baja(int id);

    }
}

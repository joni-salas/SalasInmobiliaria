using System.Data;
using System.Data.SqlClient;

namespace SalasInmobiliaria.Models
{
    public class RepositorioPropietario
    {

        string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=InmobiliariaSalasDB;Trusted_Connection=True;MultipleActiveResultSets=true";

        public RepositorioPropietario()
        {

        }

        public int Alta(Propietario p)
        {
            int res = -1;
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Propietario (Nombre, Apellido, Dni, Telefono, Email, Clave)"+
                    $"VALUES (@nombre, @apellido, @dni, @telefono, @email, @clave);"+
                    "SELECT SCOPE_IDENTITY();";// (LAST_INSERT_ID para mysql)
                using (SqlCommand comm = new SqlCommand(sql, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@nombre", p.Nombre);
                    comm.Parameters.AddWithValue("@apellido", p.Apellido);
                    comm.Parameters.AddWithValue("@dni", p.Dni);
                    comm.Parameters.AddWithValue("@telefono", p.Telefono);
                    comm.Parameters.AddWithValue("@email", p.Email);
                    comm.Parameters.AddWithValue("@clave", p.Clave);
                    conn.Open();
                    res = Convert.ToInt32(comm.ExecuteScalar()); // ultimo id de la BD
                    p.Id = res;
                    conn.Close();
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = $"DELETE FROM Propietario WHERE Id = @id";
                using(SqlCommand comm = new SqlCommand(sql, conn))
                {
                    comm.CommandType= CommandType.Text;
                    comm.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    res = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public int Modificacion(Propietario p)
        {
            int res = -1;
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Propietario SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email, Clave=@clave " +
                    $"WHERE Id = @id";
                using(SqlCommand comm = new SqlCommand(sql, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@nombre", p.Nombre);
                    comm.Parameters.AddWithValue("@apellido", p.Apellido);
                    comm.Parameters.AddWithValue("@dni", p.Dni);
                    comm.Parameters.AddWithValue("@telefono", p.Telefono);
                    comm.Parameters.AddWithValue("@email", p.Email);
                    comm.Parameters.AddWithValue("@clave", p.Clave);
                    comm.Parameters.AddWithValue("@id", p.Id);
                    conn.Open();
                    res=comm.ExecuteNonQuery();
                    conn.Close();

                }
            }
            return res;
        }

        public IList<Propietario> ObtenerTodos()
        {
            IList<Propietario> res = new List<Propietario>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, Clave" +
                    $" FROM Propietario";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Propietario p = new Propietario
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Dni = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            Clave = reader.GetString(6),
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }


        virtual public Propietario ObtenerPorId(int id)
        {
            Propietario p = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, Clave FROM Propietario" +
                    $" WHERE Id=@id";
                using (SqlCommand comm = new SqlCommand(sql, conn))
                {
                    comm.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    comm.CommandType = CommandType.Text;
                    conn.Open();
                    var reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        p = new Propietario
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Dni = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            Clave = reader.GetString(6),
                        };
                    }
                    conn.Close();
                }
            }
            return p;
        }

    }
}

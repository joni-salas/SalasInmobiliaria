using System.Data;
using System.Data.SqlClient;

namespace SalasInmobiliaria.Models
{
    public class RepositorioPropietario : RepositorioBase
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration)
        {

        }

        //string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=InmobiliariaSalasDB;Trusted_Connection=True;MultipleActiveResultSets=true";

 
        public int Alta(Propietario p)
        {
            int res = -1;
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Propietario (Nombre, Apellido, Dni, Telefono, Email, Clave, Estado)"+
                    $"VALUES (@nombre, @apellido, @dni, @telefono, @email, @clave, @estado);"+
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
                    comm.Parameters.AddWithValue("@estado", p.Estado);
                    conn.Open();
                    res = Convert.ToInt32(comm.ExecuteScalar()); // ultimo id de  la BD
                    p.Id = res;
                    conn.Close();
                }
            }
            return res;
        }

        public int Baja(Propietario p)
        {
            int res = -1;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Propietario SET Estado=@estado " +
                    $"WHERE Id = @id";
                using (SqlCommand comm = new SqlCommand(sql, conn))
                {
                    comm.CommandType= CommandType.Text;
                    comm.Parameters.AddWithValue("@estado", p.Estado);
                    comm.Parameters.AddWithValue("@id", p.Id);
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
                string sql = $"UPDATE Propietario SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email, Clave=@clave, Estado=@estado " +
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
                    comm.Parameters.AddWithValue("@estado",p.Estado);
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
                string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, Clave, Estado" +
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
                            Estado = reader.GetBoolean(7),
                        };
                        if (p.Estado)
                        {
                            res.Add(p);
                        }
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
                string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, Clave, Estado FROM Propietario" +
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
                            Estado = reader.GetBoolean(7),
                        };
                    }
                    conn.Close();
                }
            }
            return p;
        }

    }
}

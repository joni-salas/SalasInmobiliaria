using System.Data.SqlClient;
using System.Data;


namespace SalasInmobiliaria.Models
{
    public class RepositorioInquilino
    {
        string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=InmobiliariaSalasDB;Trusted_Connection=True;MultipleActiveResultSets=true";
        public RepositorioInquilino()
        {

        }

        public int Alta (Inquilino i)
        {
            int res = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO Inquilino (Nombre, Apellido, Dni, Telefono,Email, Estado)" +
                    $"VALUES (@nombre, @apellido, @dni,@telefono,@email, @estado);" +
                    "SELECT SCOPE_IDENTITY();";// devuelve el ultimo id insertado
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text; // nose para que es esto
                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@email", i.Email);
                    command.Parameters.AddWithValue("@estado", i.Estado);
                    conn.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    i.Id = res;
                    conn.Close();
                }
              
            }
            return res;
        }

        public IList<Inquilino> obtenerTodos()
        {
            var res = new List<Inquilino>();

            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"SELECT Id,Nombre,Apellido,Dni,Telefono,Email,Estado
                                   FROM Inquilino;";
                using(SqlCommand comm = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    var reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        var inquilino = new Inquilino()
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Dni = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            Estado = reader.GetBoolean(6),
                        };
                        if (inquilino.Estado)
                        {
                            res.Add(inquilino);
                        }
                        //res.Add(inquilino);

                    }
                    conn.Close();     

                }
            }
            return res;
        }

        public int Baja(Inquilino p)
        {
            int res = -1;
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Inquilino SET Estado=@estado " +
                    $"WHERE Id = @id";
                using (SqlCommand comm = new SqlCommand(sql, conn))
                {
                    comm.CommandType = CommandType.Text; // nose que hace esto
                    comm.Parameters.AddWithValue("@estado", p.Estado);
                    comm.Parameters.AddWithValue("@id", p.Id);
                    conn.Open();
                    res = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;

        }

        public int Modificacion(Inquilino i)
        {
            int res = -1;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Inquilino SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email" +
                    $" WHERE Id = @id ";
                using (SqlCommand comm = new SqlCommand(sql, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@nombre", i.Nombre);
                    comm.Parameters.AddWithValue("@apellido", i.Apellido);
                    comm.Parameters.AddWithValue("@dni", i.Dni);
                    comm.Parameters.AddWithValue("@telefono", i.Telefono);
                    comm.Parameters.AddWithValue("@email", i.Email);
                    comm.Parameters.AddWithValue("@estado", i.Estado);
                    comm.Parameters.AddWithValue("@id", i.Id);
                    conn.Open();
                    res = comm.ExecuteNonQuery();
                    conn.Close();

                }
            }
            return res;
        }

        virtual public Inquilino ObtenerPorId(int id)
        {
            Inquilino inquilino = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id,Nombre,Apellido,Dni,Telefono,Email,Estado FROM Inquilino" +
                        $" WHERE Id = @id";
                using(SqlCommand comm = new SqlCommand(sql, conn))
                {
                    comm.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    comm.CommandType=CommandType.Text;
                    conn.Open();
                    var reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        inquilino = new Inquilino
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Dni = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            Estado = reader.GetBoolean(6),

                        };
                    }
                    conn.Close();
                }
            }
            return inquilino;
        }
    }
}

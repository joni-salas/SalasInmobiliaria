using System.Data;
using System.Data.SqlClient;

namespace SalasInmobiliaria.Models
{
    public class RepositorioInmueble
    {
		string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=InmobiliariaSalasDB;Trusted_Connection=True;MultipleActiveResultSets=true";
		public RepositorioInmueble()
        {

        }

		public int Alta(Inmueble inmueble)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Inmueble (Direccion, Tipo, Ambientes, Precio, Superficie, Estado, IdPropietario) " +
					"VALUES (@Direccion, @Tipo, @Ambientes, @Precio, @Superficie, @Estado, @IdPropietario);" +
					"SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
					command.Parameters.AddWithValue("@Tipo", inmueble.Tipo);
					command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
					command.Parameters.AddWithValue("@Precio", inmueble.Precio);
					command.Parameters.AddWithValue("@Superficie", inmueble.Superficie);
					command.Parameters.AddWithValue("@Estado", inmueble.Estado);
					command.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					inmueble.Id = res;
					connection.Close();
				}
			}
			return res;
		}
		public int Baja(Inmueble i)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{

				string sql = "UPDATE Inmueble SET  Estado=@estado " +
					"WHERE Id = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@estado", "0");
					command.Parameters.AddWithValue("@id", i.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public int Modificacion(Inmueble i)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "UPDATE Inmueble SET " +
					"Direccion=@direccion, Tipo=@tipo, Ambientes=@ambientes, Precio=@precio, Superficie=@superficie, Estado=@estado, IdPropietario=@idPropietario " +
					"WHERE Id = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@direccion", i.Direccion);
					command.Parameters.AddWithValue("@tipo", i.Tipo);
					command.Parameters.AddWithValue("@ambientes", i.Ambientes);
					command.Parameters.AddWithValue("@precio", i.Precio);
					command.Parameters.AddWithValue("@superficie", i.Superficie);
					command.Parameters.AddWithValue("@estado", i.Estado);
					command.Parameters.AddWithValue("@idPropietario", i.IdPropietario);
					command.Parameters.AddWithValue("@id", i.Id);
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}


		public IList<Inmueble> ObtenerTodos()
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "SELECT i.Id, Direccion,Tipo , Ambientes, Precio, Superficie, i.Estado, i.IdPropietario," +
					" p.Nombre, p.Apellido" +
					" FROM Inmueble i INNER JOIN Propietario p ON i.IdPropietario = p.Id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble i = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Tipo = reader.GetString(2),
							Ambientes = reader.GetString(3),
							Precio = reader.GetString(4),
							Superficie = reader.GetString(5),
							Estado = reader.GetString(6),
							IdPropietario = reader.GetInt32(7),
							prop = new Propietario
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}
						};
						
                        if (i.Estado == "1")
                        {
							res.Add(i);
						}
						
					}
					connection.Close();
				}
			}
			return res;
		}

		public Inmueble ObtenerPorId(int id)
		{
			Inmueble i = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, Direccion,Tipo, Ambientes, Precio, Superficie, i.Estado, IdPropietario, p.Nombre, p.Apellido" +
					$" FROM Inmueble i INNER JOIN Propietario p ON i.IdPropietario = p.Id" +
					$" WHERE i.Id=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						i = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Tipo = reader.GetString(2),
							Ambientes = reader.GetString(3),
							Precio = reader.GetString(4),
							Superficie = reader.GetString(5),
							Estado = reader.GetString(6),
							IdPropietario = reader.GetInt32(7),
							prop = new Propietario
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}
						};
					}
					connection.Close();
				}
			}
			return i;
		}

		public IList<Inmueble> BuscarPorPropietario(int idPropietario)
		{
			List<Inmueble> res = new List<Inmueble>();
			Inmueble i = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, Direccion,Tipo, Ambientes, Precio, Superficie, i.Estado, IdPropietario, p.Nombre, p.Apellido" +
					$" FROM Inmueble i INNER JOIN Propietario p ON i.IdPropietario = p.Id" +
					$" WHERE p.Id=@idPropietario";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@idPropietario", SqlDbType.Int).Value = idPropietario;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						i = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Tipo = reader.GetString(2),
							Ambientes = reader.GetString(3),
							Precio = reader.GetString(4),
							Superficie = reader.GetString(5),
							Estado = reader.GetString(6),
							IdPropietario = reader.GetInt32(7),
							prop = new Propietario
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}
						};
						res.Add(i);
					}
					connection.Close();
				}
			}
			return res;
		}


		public IList<Inmueble> ObtenerActivosInactivosAlquilados()
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "SELECT i.Id, Direccion,Tipo , Ambientes, Precio, Superficie, i.Estado, i.IdPropietario," +
					" p.Nombre, p.Apellido" +
					" FROM Inmueble i INNER JOIN Propietario p ON i.IdPropietario = p.Id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble i = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Tipo = reader.GetString(2),
							Ambientes = reader.GetString(3),
							Precio = reader.GetString(4),
							Superficie = reader.GetString(5),
							Estado = reader.GetString(6),
							IdPropietario = reader.GetInt32(7),
							prop = new Propietario
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}
						};

						
						 res.Add(i);
						

					}
					connection.Close();
				}
			}
			return res;
		}



		public IList<Inmueble> ObtenerAlquilados()
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "SELECT i.Id, Direccion,Tipo , Ambientes, Precio, Superficie, i.Estado, i.IdPropietario," +
					" p.Nombre, p.Apellido" +
					" FROM Inmueble i INNER JOIN Propietario p ON i.IdPropietario = p.Id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble i = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Tipo = reader.GetString(2),
							Ambientes = reader.GetString(3),
							Precio = reader.GetString(4),
							Superficie = reader.GetString(5),
							Estado = reader.GetString(6),
							IdPropietario = reader.GetInt32(7),
							prop = new Propietario
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}
						};

                        if (i.Estado == "Alquilado")
                        {
							res.Add(i);
						}
						
					}
					connection.Close();
				}
			}
			return res;
		}


	}
}

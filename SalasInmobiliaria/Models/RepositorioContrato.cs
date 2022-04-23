using System.Data;
using System.Data.SqlClient;

namespace SalasInmobiliaria.Models
{
    public class RepositorioContrato : RepositorioBase
    {
        public RepositorioContrato(IConfiguration configuration) :base(configuration) 
        {

        }
        //string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=InmobiliariaSalasDB;Trusted_Connection=True;MultipleActiveResultSets=true";


		public int Alta(Contrato contrato)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Contrato (NombreGarante, TelefonoGarante, DniGarante, Monto, FechaInicio, FechaFin, IdInquilino, IdInmueble) " +
					"VALUES (@NombreGarante, @TelefonoGarante, @DniGarante, @Monto, @FechaInicio, @FechaFin, @IdInquilino, @IdInmueble);" +
					"SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@NombreGarante", contrato.NombreGarante);
					command.Parameters.AddWithValue("@TelefonoGarante", contrato.TelefonoGarante);
					command.Parameters.AddWithValue("@DniGarante", contrato.DniGarante);
					command.Parameters.AddWithValue("@Monto", contrato.Monto);
					command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
					command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
					command.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
					command.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					contrato.Id = res;
					connection.Close();
				}
			}
			return res;
		}

		public int Baja(Contrato contrato)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{

				string sql = "DELETE FROM Contrato WHERE Id=@Id;";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@Id", contrato.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}


		public int Modificacion(Contrato contrato)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "UPDATE Contrato SET " +
					"NombreGarante=@NombreGarante, TelefonoGarante=@TelefonoGarante, DniGarante=@DniGarante, Monto=@Monto, FechaInicio=@FechaInicio, FechaFin=@FechaFin " +
					"WHERE Id = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@NombreGarante", contrato.NombreGarante);
					command.Parameters.AddWithValue("@TelefonoGarante", contrato.TelefonoGarante);
					command.Parameters.AddWithValue("@DniGarante", contrato.DniGarante);
					command.Parameters.AddWithValue("@Monto", contrato.Monto);
					command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
					command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
					command.Parameters.AddWithValue("@id", contrato.Id);
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Contrato> ObtenerTodos()
		{
			IList<Contrato> res = new List<Contrato>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "SELECT c.Id, NombreGarante,TelefonoGarante , DniGarante, Monto, FechaInicio, FechaFin, IdInquilino, IdInmueble," +
					" i.Nombre, i.Apellido, inmu.Direccion" +
					" FROM Contrato c INNER JOIN Inquilino i ON c.IdInquilino = i.Id" +
					" INNER JOIN Inmueble inmu ON c.IdInmueble = inmu.Id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato contrato = new Contrato
						{
							Id = reader.GetInt32(0),
							NombreGarante = reader.GetString(1),
							TelefonoGarante = reader.GetString(2),
							DniGarante = reader.GetString(3),
							Monto = reader.GetString(4),
							FechaInicio = reader.GetDateTime(5), //datetime
							FechaFin = reader.GetDateTime(6),
							IdInquilino = reader.GetInt32(7),
							IdInmueble = reader.GetInt32(8),
							Inqui = new Inquilino
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(9),
								Apellido = reader.GetString(10),
							},
							Inmu = new Inmueble
                            {
								Id = reader.GetInt32(8),
								Direccion = reader.GetString(11),
                            }
						};

						res.Add(contrato);

					}
					connection.Close();
				}
			}
			return res;
		}


		public Contrato ObtenerPorId(int id)
		{
			Contrato contrato = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{

				string sql = $"SELECT c.Id, NombreGarante,TelefonoGarante , DniGarante, Monto, FechaInicio, FechaFin, IdInquilino, IdInmueble," +
					" i.Nombre, i.Apellido, inmu.Direccion" +
					" FROM Contrato c INNER JOIN Inquilino i ON c.IdInquilino = i.Id" +
					" INNER JOIN Inmueble inmu ON c.IdInmueble = inmu.Id"+
					$" WHERE c.Id=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						contrato = new Contrato
						{
							Id = reader.GetInt32(0),
							NombreGarante = reader.GetString(1),
							TelefonoGarante = reader.GetString(2),
							DniGarante = reader.GetString(3),
							Monto = reader.GetString(4),
							FechaInicio = reader.GetDateTime(5), //datetime
							FechaFin = reader.GetDateTime(6),
							IdInquilino = reader.GetInt32(7),
							IdInmueble = reader.GetInt32(8),
							Inqui = new Inquilino
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(9),
								Apellido = reader.GetString(10),
							},
							Inmu = new Inmueble
							{
								Id = reader.GetInt32(8),
								Direccion = reader.GetString(11),
							}
						};
					}
					connection.Close();
				}
			}
			return contrato;
		}

		public IList<Contrato> BuscarPorInquilino(int IdInquilino)
		{
			List<Contrato> res = new List<Contrato>();
			Contrato contrato = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT c.Id, NombreGarante,TelefonoGarante , DniGarante, Monto, FechaInicio, FechaFin, IdInquilino, IdInmueble ," +
					$" i.Nombre, i.Apellido, inmu.Direccion" +
					$" FROM Contrato c INNER JOIN Inquilino i ON c.IdInquilino = i.Id" +
					$" INNER JOIN Inmueble inmu ON c.IdInmueble = inmu.Id" +
					$" WHERE i.Id=@IdInquilino";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@IdInquilino", SqlDbType.Int).Value = IdInquilino;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						contrato = new Contrato
						{
							Id = reader.GetInt32(0),
							NombreGarante = reader.GetString(1),
							TelefonoGarante = reader.GetString(2),
							DniGarante = reader.GetString(3),
							Monto = reader.GetString(4),
							FechaInicio = reader.GetDateTime(5), //datetime
							FechaFin = reader.GetDateTime(6),
							IdInquilino = reader.GetInt32(7),
							IdInmueble = reader.GetInt32(8),
							Inqui = new Inquilino
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(9),
								Apellido = reader.GetString(10),
							},
							Inmu = new Inmueble
							{
								Id = reader.GetInt32(8),
								Direccion = reader.GetString(11),
							}
						};
						res.Add(contrato);
					}
					connection.Close();
				}
			}
			return res;
		}

		public IList<Contrato> BuscarPorInmueble(int IdInmueble)
		{
			List<Contrato> res = new List<Contrato>();
			Contrato contrato = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT c.Id, NombreGarante,TelefonoGarante , DniGarante, Monto, FechaInicio, FechaFin, IdInquilino, IdInmueble," +
					" i.Nombre, i.Apellido, inmu.Direccion" +
					" FROM Contrato c INNER JOIN Inquilino i ON c.IdInquilino = i.Id" +
					" INNER JOIN Inmueble inmu ON c.IdInmueble = inmu.Id" +
					$" WHERE inmu.Id=@IdInmueble";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@IdInmueble", SqlDbType.Int).Value = IdInmueble;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						contrato = new Contrato
						{
							Id = reader.GetInt32(0),
							NombreGarante = reader.GetString(1),
							TelefonoGarante = reader.GetString(2),
							DniGarante = reader.GetString(3),
							Monto = reader.GetString(4),
							FechaInicio = reader.GetDateTime(5), //datetime
							FechaFin = reader.GetDateTime(6),
							IdInquilino = reader.GetInt32(7),
							IdInmueble = reader.GetInt32(8),
							Inqui = new Inquilino
							{
								Id = reader.GetInt32(7),
								Nombre = reader.GetString(9),
								Apellido = reader.GetString(10),
							},
							Inmu = new Inmueble
							{
								Id = reader.GetInt32(8),
								Direccion = reader.GetString(11),
							}
						};
						res.Add(contrato);
					}
					connection.Close();
				}
			}
			return res;
		}



	}



}


using System.Data;
using System.Data.SqlClient;

namespace SalasInmobiliaria.Models
{
    public class RepositorioPago : RepositorioBase
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
		{

        }

        //string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=InmobiliariaSalasDB;Trusted_Connection=True;MultipleActiveResultSets=true";

		public int Alta(Pago pago)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Pago (Importe, NPago, Fecha, IdContrato) " +
					"VALUES (@Importe, @NPago, @Fecha, @IdContrato);" +
					"SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@Importe", pago.Importe);
					command.Parameters.AddWithValue("@NPago", pago.NPago);
					command.Parameters.AddWithValue("@Fecha", pago.Fecha);
					command.Parameters.AddWithValue("@IdContrato", pago.IdContrato);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					pago.Id = res;
					connection.Close();
				}
			}
			return res;
		}

		public int Baja(Pago pago)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{

				string sql = "DELETE FROM Pago WHERE Id=@Id;";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@Id", pago.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public int Modificacion(Pago pago)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "UPDATE Pago SET " +
					"Importe=@Importe, NPago=@NPago, Fecha=@Fecha, IdContrato=@IdContrato " +
					"WHERE Id = @Id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@Importe", pago.Importe);
					command.Parameters.AddWithValue("@NPago", pago.NPago);
					command.Parameters.AddWithValue("@Fecha", pago.Fecha);
					command.Parameters.AddWithValue("@IdContrato", pago.IdContrato);
					command.Parameters.AddWithValue("@Id", pago.Id);
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Pago> ObtenerTodos()
		{
			IList<Pago> res = new List<Pago>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "SELECT p.Id, Importe,NPago , Fecha, IdContrato" +
					" FROM Pago p INNER JOIN Contrato c ON p.IdContrato = c.Id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Pago pago = new Pago
						{
							Id = reader.GetInt32(0),
							Importe = reader.GetDouble(1),
							NPago = reader.GetInt32(2),
							Fecha = reader.GetDateTime(3),
							IdContrato = reader.GetInt32(4),
							
							Cont = new Contrato
							{
								Id = reader.GetInt32(4),
							},
						};

						res.Add(pago);

					}
					connection.Close();
				}
			}
			return res;
		}


		public Pago ObtenerPorId(int id)
		{
			Pago pago = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{

				string sql = $"SELECT p.Id, Importe,NPago , Fecha, IdContrato" +
					" FROM Pago p INNER JOIN Contrato c ON p.IdContrato = c.Id" +
					$" WHERE p.Id=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						pago = new Pago
						{
							Id = reader.GetInt32(0),
							Importe = reader.GetDouble(1),
							NPago = reader.GetInt32(2),
							Fecha = reader.GetDateTime(3),
							IdContrato = reader.GetInt32(4),

							Cont = new Contrato
							{
								Id = reader.GetInt32(4),
							},
						};
					}
					connection.Close();
				}
			}
			return pago;
		}


		public IList<Pago> BuscarPorContrato(int IdContrato)
		{
			List<Pago> res = new List<Pago>();
			Pago pago = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT p.Id, Importe,NPago , Fecha, IdContrato " +
					$" FROM Pago p INNER JOIN Contrato c ON p.IdContrato = c.Id" +
					$" WHERE c.Id=@IdContrato";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@IdContrato", SqlDbType.Int).Value = IdContrato;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						pago = new Pago
						{
							Id = reader.GetInt32(0),
							Importe = reader.GetDouble(1),
							NPago = reader.GetInt32(2),
							Fecha = reader.GetDateTime(3),
							IdContrato = reader.GetInt32(4),

							Cont = new Contrato
							{
								Id = reader.GetInt32(4),
							},
						};
						res.Add(pago);
					}
					connection.Close();
				}
			}
			return res;
		}

	}
}

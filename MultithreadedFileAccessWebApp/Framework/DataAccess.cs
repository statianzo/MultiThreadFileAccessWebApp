using System;
using System.Data.SqlServerCe;
using System.IO;

namespace MultithreadedFileAccessWebApp.Framework
{
	public class DataAccess
	{
		private static string _fileName;
		private static string _connectionString;

		public static void StartUp()
		{
			var storageDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "storage");
			if (!Directory.Exists(storageDir))
				Directory.CreateDirectory(storageDir);
			AppDomain.CurrentDomain.SetData("SQLServerCompactEditionUnderWebHosting", true);
			_fileName = Path.Combine(storageDir, "WebAppDatabse.sdf");
			_connectionString = "DataSource=" + _fileName;

			CreateDatabase();
			CreateTables();
			GenerateData();
		}

		public static string GetDescription(string name)
		{
			using (var connection = new SqlCeConnection(_connectionString))
			{
				connection.Open();
				string sql = "select description from Foo where name=@name";
				var command = new SqlCeCommand(sql, connection);
				command.Parameters.AddWithValue("@name", name);
				object result = command.ExecuteScalar();
				return result != null ? result.ToString() : null;
			}
		}

		private static void GenerateData()
		{
			using (var connection = new SqlCeConnection(_connectionString))
			{
				connection.Open();

				string sql = "insert into Foo(name, description) " +
				             "values(@name, @description)";
				for (int i = 0; i < 1000; i++)
				{
					using (var command = new SqlCeCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@name", "FileName" + i);
						command.Parameters.AddWithValue("@description", new string('a', 90) + i);
						command.ExecuteNonQuery();
					}
				}
			}
		}

		private static void CreateDatabase()
		{
			if (File.Exists(_fileName))
				File.Delete(_fileName);
			using (var engine = new SqlCeEngine(_connectionString))
				engine.CreateDatabase();
		}

		private static void CreateTables()
		{
			using (var connection = new SqlCeConnection(_connectionString))
			{
				connection.Open();
				string sql = "create table Foo(" +
				             "name nvarchar (40) not null, " +
				             "description nvarchar (100) not null)";
				using (var command = new SqlCeCommand(sql, connection))
					command.ExecuteNonQuery();
			}
		}
	}
}
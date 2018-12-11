using System;
using Npgsql;
namespace Garlic.Database
{
    public class PostgreSqlServer : IServer
    {
        private readonly string _connectionString;

        public PostgreSqlServer(string connectionString)
        {
            _connectionString = connectionString;
        }

        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public bool VerifyConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Batch(string batch, bool inTransaction)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var transaction = inTransaction ? connection.BeginTransaction() : null;
                var commands = CommandParser.EndsWith(";", batch);
                foreach (var c in commands)
                {
                    try
                    {
                        var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        command.CommandText = c;
                        command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        Console.Error.WriteLine("The sql command failed:");
                        Console.Error.WriteLine(c);
                        if (transaction != null)
                        {
                            transaction.Rollback();
                        }
                        throw;
                    }
                }
                if (transaction != null)
                {
                    transaction.Commit();
                }
            }
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public T GetSingle<T>(SqlCommand sqlCommand)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                {
                    var command = connection.CreateCommand();
                    command.CommandText = sqlCommand.CommandText;
                    if (null != sqlCommand.Parameters) foreach (var parameter in sqlCommand.Parameters)
                        {
                            command.Parameters.Add(new NpgsqlParameter(parameter.Key, parameter.Value));
                        }
                    using (var reader = command.ExecuteReader())
                    {
                        object value = null;
                        if (reader.Read())
                            value = reader.IsDBNull(0) ? null : reader.GetValue(0);
                        return (T)(value);
                    }
                }
            }
        }
    }
}
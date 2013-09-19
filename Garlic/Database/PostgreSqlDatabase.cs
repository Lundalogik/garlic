using System;
using System.Collections.Generic;
using Npgsql;

namespace Garlic.Database
{
    public class PostgreSqlDatabase : IDatabase
    {
        private readonly string _connectionString;
        private readonly string _databaseName;

        public PostgreSqlDatabase(string connectionString, string databaseName)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
        }

        private NpgsqlConnection GetConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.ChangeDatabase(_databaseName);
            return connection;
        }

        public bool VerifyConnection()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.ChangeDatabase(_databaseName);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public void Batch(Func<NpgsqlConnection, IEnumerable<NpgsqlCommand>> commands, bool inTransaction = true)
        {
            using (var connection = GetConnection())
            {
                var transaction = inTransaction ? connection.BeginTransaction() : null;
                foreach (var command in commands(connection))
                {
                    try
                    {
                        command.CommandTimeout = 60 * 10; // Ten minutes
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        Console.Error.WriteLine("The sql command failed:");
                        Console.Error.WriteLine(command.CommandText);
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

        public void Batch(string sqlcommands, bool inTransaction = true, params SqlParam[] parameters)
        {
            var commands = CommandParser.EndsWith(";", sqlcommands);
            Batch(conn =>
            {
                var npgsqlCommands = new List<NpgsqlCommand>();
                foreach (var sql in commands)
                {
                    var npgsqlCommand = conn.CreateCommand();
                    foreach (var parameter in parameters)
                    {
                        npgsqlCommand.Parameters.Add(new NpgsqlParameter(parameter.Key, parameter.Value));
                    }
                    npgsqlCommand.CommandText = sql;
                    npgsqlCommands.Add(npgsqlCommand);
                }
                return npgsqlCommands;
            }, inTransaction);
        }

        public void Batch(IEnumerable<SqlCommand> commands)
        {
            Batch(conn =>
            {
                var npgsqlCommands = new List<NpgsqlCommand>();
                foreach (var c in commands)
                {
                    var npgsqlCommand = conn.CreateCommand();
                    if (null != c.Parameters) foreach (var parameter in c.Parameters)
                        {
                            npgsqlCommand.Parameters.Add(new NpgsqlParameter(parameter.Key, parameter.Value));
                        }
                    npgsqlCommand.CommandText = c.CommandText;
                    npgsqlCommands.Add(npgsqlCommand);
                }
                return npgsqlCommands;
            });
        }

        public T GetSingle<T>(SqlCommand sqlCommand)
        {
            //NOTE: pasted from PostgreSqlServer.GetSingle
            using (var connection = GetConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = sqlCommand.CommandText;
                if (null != sqlCommand.Parameters)
                    foreach (var parameter in sqlCommand.Parameters)
                    {
                        command.Parameters.Add(new NpgsqlParameter(parameter.Key, parameter.Value));
                    }
                using (var reader = command.ExecuteReader())
                {
                    object value = null;
                    if (reader.Read())
                        value = reader.IsDBNull(0) ? null : reader.GetValue(0);
                    return (T) (value);
                }
            }
        }

        public string Name
        {
            get { return _databaseName; }
        }

        public void InitRevision()
        {
            Batch("CREATE TABLE revision (name VARCHAR(150))");
            Batch("INSERT INTO revision (name) VALUES ('')");
        }

        public void SetRevision(string name)
        {
            Batch("UPDATE revision SET name = '" + name + "'");
        }

        public string GetRevision()
        {
            return GetScalar("SELECT name FROM revision").ToString();
        }

        public object GetScalar(string sql)
        {
            using (var connection = GetConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = sql;
                return command.ExecuteScalar();
            }
        }
    }
}
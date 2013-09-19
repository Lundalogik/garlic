using System;
using System.Collections.Generic;
using System.Linq;

namespace Garlic
{
    public enum DatabaseBrand
    {
        PostgreSql,
    }
    public interface IDatabase
    {
        bool VerifyConnection();
        void Batch(string sqlcommands, bool inTransaction = true, params SqlParam[] parameters);
        void Batch(IEnumerable<SqlCommand> commands);
        T GetSingle<T>(SqlCommand sqlCommand);
        string Name { get; }

        void InitRevision();
        void SetRevision(string name);
        string GetRevision();
    }
    public static class DatabaseExtensions
    {
        public static T GetSingle<T>(this IDatabase self, string sqlCommand)
        {
            return self.GetSingle<T>(new SqlCommand { CommandText = sqlCommand });
        }

        public static void Sql(this IDatabase database, string script, params object[] f)
        {
            script = f != null ? String.Format(script, f) : script;
            database.Batch(script);
        }
        public static void Sql(this IDatabase database, SqlCommand script, params object[] f)
        {
            var cmdtext = f != null ? String.Format(script.CommandText, f) : script.CommandText;
            var sqlCommand = new SqlCommand { CommandText = cmdtext, Parameters = script.Parameters.ToArray() };
            database.Batch(new[] { sqlCommand });
        }
    }
    public class SqlCommand
    {
        public string CommandText { get; set; }
        public SqlParam[] Parameters { get; set; }
    }

    public class SqlParam
    {
        public string Key { get; private set; }
        public object Value { get; private set; }

        public SqlParam(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}


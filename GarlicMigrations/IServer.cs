namespace Garlic
{    
    /// <summary>
    /// No explicit database
    /// </summary>
    public interface IServer
    {
        bool VerifyConnection();
        void Batch(string batch, bool inTransaction = true);
        string ConnectionString { get; }
        T GetSingle<T>(SqlCommand sqlCommand);
    }

    public static class ServerExtensions
    {
        public static T GetSingle<T>(this IServer self, string sqlCommand)
        {
            return self.GetSingle<T>(new SqlCommand { CommandText = sqlCommand });
        }
    }
}
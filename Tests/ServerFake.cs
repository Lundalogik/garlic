using System;
using Garlic;

namespace Tests
{
    public class ServerFake : IServer
    {
        public bool VerifyConnection()
        {
            return true;
        }

        public void Batch(string batch, bool inTransaction)
        {
            throw new NotImplementedException();
        }

        public string ConnectionString
        {
            get { return "Faked connectionstring"; }
        }

        public T GetSingle<T>(SqlCommand commandText)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using Garlic;

namespace Tests
{
    public class DatabaseFake : IDatabase
    {
        private string _revisionName;

        public bool VerifyConnection()
        {
            return VerifyConnectionValue;
        }

        public void Batch(string sqlcommands, bool inTransaction = true, params SqlParam[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Batch(IEnumerable<SqlCommand> commands)
        {
            throw new NotImplementedException();
        }

        public T GetSingle<T>(SqlCommand commandText)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { return "Faked databasename"; }
        }

        public bool VerifyConnectionValue
        {
            get; set;
        }

        public void InitRevision()
        {
        }

        public void SetRevision(string name)
        {
            _revisionName = name;
        }

        public string GetRevision()
        {
            return _revisionName;
        }
    }
}
using System;
using Garlic.Database;

namespace Garlic
{
    public static class Factory
    {
        public static IDatabase GetDatabase(DatabaseBrand databaseBrand, string connectionString, string databaseName)
        {
            switch (databaseBrand)
            {
                case DatabaseBrand.PostgreSql:
                    return new PostgreSqlDatabase(connectionString, databaseName);
                default:
                    throw new ArgumentOutOfRangeException("databaseBrand");
            }            
        }

        public static IServer GetServer(DatabaseBrand databaseBrand, string connectionString)
        {
            switch (databaseBrand)
            {
                case DatabaseBrand.PostgreSql:
                    return new PostgreSqlServer(connectionString);
                default:
                    throw new ArgumentOutOfRangeException("databaseBrand");
            }
        }

        public static Target GetTarget(DatabaseBrand databaseBrand, string connectionString, string databaseName)
        {
            return new Target(GetServer(databaseBrand, connectionString), GetDatabase(databaseBrand, connectionString, databaseName));
        }
    }
}
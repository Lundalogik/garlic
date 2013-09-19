using System;
using System.Collections.Generic;
using System.Linq;

namespace Garlic
{
    public class Target
    {
        private readonly IServer _server;
        private readonly IDatabase _database;
        private readonly IList<Revision> _revisions = new List<Revision>();

        public delegate void EventHandler(IServer server, IDatabase database);
        public event EventHandler AfterSync;

        private void InvokeAfterSync()
        {
            EventHandler handler = AfterSync;
            if (handler != null) handler(_server, _database);
        }

        public Target(IServer server, IDatabase database)
        {
            _server = server;
            _database = database;
        }

        private void Create()
        {
            Logger.Info("Creating new database {0}", _database.Name);

            // Try applying the first script as a boot
            var revision = _revisions.First();
            try
            {
                revision.Apply(_server, _database);                
            }
            catch (Exception e)
            {
                Logger.Error("Failed to create new database with revision '" + revision.Name + "'");
                Logger.Error("Error: " + e.Message);
                throw;
            }

            if (!_database.VerifyConnection()) throw new Exception();
            _database.InitRevision();
            _database.SetRevision(revision.Name);            
        }

        public void Sync(bool createDatabase = true)
        {
            Logger.Info("Trying to connect to server");
            if (!_server.VerifyConnection())
            {
                Logger.Error("Failed to connect to server");
                throw new Exception();
            }
            Logger.Info("Connected to server.");

            Logger.Info("Trying to open database " + _database.Name);
            if (!_database.VerifyConnection())
            {
                if (createDatabase)
                {
                    Create();                    
                }
                else
                {
                    Logger.Error("Database {0} does not exist or can not be accessed", _database.Name);
                    throw new GarlicException("Database cannot be opened");
                }
            }
            else if (createDatabase)
            {
                Logger.Error("Cannot create database. Database {0} already exists.", _database.Name);
                throw new GarlicException("Database already exists");
            }

            var currentRevisionName = _database.GetRevision();

            // Check that current revision exists
            if (_revisions.Where(r => r.Name == currentRevisionName).Count() == 0)
            {
                Logger.Error("Current revision {0} can not be found!", currentRevisionName);
                throw new RevisionNotFoundException(currentRevisionName);
            }

            var revisionsToApply = _revisions
                .SkipWhile(r => r.Name != currentRevisionName)
                .Skip(1);

            Logger.Info("Found {0} revisions to apply", revisionsToApply.Count());

            foreach (var revision in revisionsToApply)
            {
                try
                {
                    Logger.Info("Applying revision '{0}'", revision.Name);
                    revision.Apply(_server, _database);
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to run revision '" + revision.Name + "'");
                    Logger.Error("Error: " + e.Message);
                    throw;
                }
                _database.SetRevision(revision.Name);
            }

            InvokeAfterSync();
        }

        public void AddRevision(Revision revision)
        {
            if (_revisions.Any(r => r.Name == revision.Name))
            {
                throw new GarlicException(string.Format("Duplicate revision name found: {0}", revision.Name));
            }

            _revisions.Add(revision);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;

namespace Garlic
{
    public class Target
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IServer _server;
        private readonly IDatabase _database;
        private readonly IList<Revision> _revisions = new List<Revision>();

        public delegate void EventHandler(IServer server, IDatabase database);
        public event EventHandler AfterSync;

        private void InvokeAfterSync()
        {
            var handler = AfterSync;
            handler?.Invoke(_server, _database);
        }

        public Target(IServer server, IDatabase database)
        {
            _server = server;
            _database = database;
        }

        private void Create()
        {
            Log.Info($"Creating new database {_database.Name}");

            // Try applying the first script as a boot
            var revision = _revisions.First();
            try
            {
                revision.Apply(_server, _database);
            }
            catch (Exception e)
            {
                Log.Error("Failed to create new database with revision '" + revision.Name + "'");
                Log.Error("Error: ", e);
                throw;
            }

            if (!_database.VerifyConnection()) throw new Exception();
            _database.InitRevision();
            _database.SetRevision(revision.Name);
        }

        private IList<Revision> GetRevisionsToApply(string currentRevisionName)
        {
            // Check that current revision exists
            var current = _revisions.SingleOrDefault(r => r.Name == currentRevisionName);
            if (current == null)
            {
                Log.Error($"Current revision {currentRevisionName} can not be found!");
                throw new RevisionNotFoundException(currentRevisionName);
            }

            return _revisions
                .SkipWhile(r => r.Name != current.Name)
                .Skip(1)
                .ToList();
        }

        public void Sync(bool createDatabase = true)
        {
            Log.Info("Trying to connect to server");
            if (!_server.VerifyConnection())
            {
                Log.Error("Failed to connect to server");
                throw new Exception();
            }
            Log.Info("Connected to server.");

            Log.Info("Trying to open database " + _database.Name);
            if (!_database.VerifyConnection())
            {
                if (createDatabase)
                {
                    Create();
                }
                else
                {
                    Log.Error($"Database {_database.Name} does not exist or can not be accessed");
                    throw new GarlicException("Database cannot be opened");
                }
            }
            else if (createDatabase)
            {
                Log.Error($"Cannot create database. Database {_database.Name} already exists.");
                throw new GarlicException("Database already exists");
            }

            var currentRevisionName = _database.GetRevision();
            var revisionsToApply = GetRevisionsToApply(currentRevisionName);

            Log.Info($"Found {revisionsToApply.Count} revisions to apply");

            foreach (var revision in revisionsToApply)
            {
                try
                {
                    Log.Info($"Applying revision '{revision.Name}'");
                    revision.Apply(_server, _database);
                }
                catch (Exception e)
                {
                    Log.Error("Failed to run revision '" + revision.Name + "'");
                    Log.Error("Error: ", e);
                    throw;
                }
                _database.SetRevision(revision.Name);
            }

            InvokeAfterSync();
        }

        public IEnumerable<Revision>  GetRevisionsToApply()
        {
            Log.Info("Trying to open database " + _database.Name);
            if (!_database.VerifyConnection())
            {
                Log.Error($"Database {_database.Name} does not exist or can not be accessed");
                throw new GarlicException("Database cannot be opened");
            }

            var currentRevisionName = _database.GetRevision();
            return GetRevisionsToApply(currentRevisionName);
        }

        public void AddRevision(Revision revision)
        {
            if (_revisions.Any(r => r.Name == revision.Name))
            {
                throw new GarlicException($"Duplicate revision name found: {revision.Name}");
            }

            _revisions.Add(revision);
        }
    }
}


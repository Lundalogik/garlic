using System;
using System.IO;
using Garlic;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class TargetTests
    {
// ReSharper disable InconsistentNaming
        [Test]
        public void Sync_with_create_flag_should_throw_when_database_can_be_opened()
        {
            var target = new Target(new ServerFake(), new DatabaseFake { VerifyConnectionValue = true});

            Assert.Throws<GarlicException>(() => target.Sync());
        }

        [Test]
        public void Sync_with_create_flag_should_use_first_revision_to_create_database()
        {
            var database = new DatabaseFake { VerifyConnectionValue = false };
            var target = new Target(new ServerFake(), database);
            var appliedFirstRevision = false;
            target.AddRevision(new Revision("create", (s, d) =>
                {
                    appliedFirstRevision = true;
                    database.VerifyConnectionValue = true;
                }));

            target.Sync();

            Assert.True(appliedFirstRevision);
        }

        [Test]
        public void Sync_with_create_flag_should_create_initial_revision()
        {
            var database = new DatabaseFake { VerifyConnectionValue = false };
            var target = new Target(new ServerFake(), database);
            target.AddRevision(new Revision("create", (s, d) => database.VerifyConnectionValue = true));
            target.Sync();

            Assert.That(database.GetRevision(), Is.EqualTo("create"));
        }

        [Test]
        public void Sync_without_create_flag_should_throw_when_database_can_not_be_opened()
        {
            var target = new Target(new ServerFake(), new DatabaseFake { VerifyConnectionValue = false });

            Assert.Throws<GarlicException>(() => target.Sync(createDatabase: false));
        }

        [Test]
        public void Sync_with_revision_that_can_not_be_found_should_throw()
        {
            var database = new DatabaseFake { VerifyConnectionValue = false };
            database.SetRevision("Unknown revision");
            var target = new Target(new ServerFake(), new DatabaseFake { VerifyConnectionValue = true });
            target.AddRevision(new Revision("Something", null));

            Assert.Throws<RevisionNotFoundException>(() => target.Sync(createDatabase: false));
        }

        [Test]
        public void Sync_with_revision_should_apply_revisions_after()
        {
            var database = new DatabaseFake { VerifyConnectionValue = true };
            database.SetRevision("Current");
            var target = new Target(new ServerFake(), database);
            target.AddRevision(new Revision("First", null));
            target.AddRevision(new Revision("Current", null));
            bool applied1 = false;
            target.AddRevision(new Revision("After 1", (s, d) => applied1 = true ));
            bool applied2 = false;
            target.AddRevision(new Revision("After 2", (s, d) => applied2 = true ));

            target.Sync(false);

            Assert.True(applied1 && applied2);
        }

        [Test]
        public void Sync_should_set_revision_after_successful_apply()
        {
            var database = new DatabaseFake { VerifyConnectionValue = true };
            database.SetRevision("Current");
            var target = new Target(new ServerFake(), database);
            target.AddRevision(new Revision("First", null));
            target.AddRevision(new Revision("Current", null));
            target.AddRevision(new Revision("After", (s, d) => d = d));

            target.Sync(false);

            Assert.That(database.GetRevision(), Is.EqualTo("After"));
        }

        [Test]
        public void Sync_should_trigger_after_sync_event()
        {
            var database = new DatabaseFake { VerifyConnectionValue = true };
            database.SetRevision("Current");
            var target = new Target(new ServerFake(), database);
            target.AddRevision(new Revision("Current", null));
            target.AddRevision(new Revision("After", (s, d) => d = d ));
            var afterSynced = false;
            target.AfterSync += (s, d) => afterSynced = true;

            target.Sync(false);

            Assert.True(afterSynced);
        }

        [Test]
        public void Add_revision_with_duplicate_name_should_throw()
        {
            var target = new Target(new ServerFake(), new DatabaseFake { VerifyConnectionValue = false });

            target.AddRevision(new Revision("name", null));

            Assert.Throws<GarlicException>(() => target.AddRevision(new Revision("name", null)));
        }

        [Test]
        public void Sync_should_call_down_when_up_throws_and_rethrow()
        {
            var database = new DatabaseFake { VerifyConnectionValue = true };
            database.SetRevision("Current");
            var target = new Target(new ServerFake(), database);
            target.AddRevision(new Revision("Current", null));
            var downCalled = false;
            target.AddRevision(new Revision("Throws", (s, d) => { throw new Exception(); }, (s, d) => downCalled = true));

            Assert.Throws<Exception>(() => target.Sync(false));
            Assert.True(downCalled);
        }

// ReSharper restore InconsistentNaming
    }
}

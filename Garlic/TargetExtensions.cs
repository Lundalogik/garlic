using System;

namespace Garlic
{
    public static class TargetExtensions
    {
        public static void Sql(this Target target, string name, string script, params object[] f)
        {
            script = f != null ? string.Format(script, f) : script;
            target.AddRevision(new Revision(name, (server, database) => database.Batch(script)));
        }

        public static void SqlInServerContext(this Target target, string name, string script, params object[] f)
        {
            script = f != null ? string.Format(script, f) : script;
            target.AddRevision(new Revision(name, (server, database) => server.Batch(script)));
        }

        public static void SqlInServerContextNoTrans(this Target target, string name, string script, params object[] f)
        {
            script = f != null ? string.Format(script, f) : script;
            target.AddRevision(new Revision(name, (server, database) => server.Batch(script, false)));
        }

        public static void Code(this Target target, string name, Action<IServer, IDatabase> up)
        {
            target.AddRevision(new Revision(name, WrapWithTransaction(up)));
        }

        private static Action<IServer, IDatabase> WrapWithTransaction(Action<IServer, IDatabase> action)
        {
            return (server, database) => {
                {
                    action(server, database); 
                
                }
            };
        }
    }
}

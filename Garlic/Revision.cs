using System;

namespace Garlic
{
    public class Revision
    {
        private readonly Action<IServer, IDatabase> _up;
        private readonly Action<IServer, IDatabase> _down;

        public Revision(string name, Action<IServer, IDatabase> up)
        {
            Name = name;
            _up = up;
            _down = null;
        }

        public Revision(string name, Action<IServer, IDatabase> up, Action<IServer, IDatabase> down)
        {
            Name = name;
            _up = up;
            _down = down;
        }

        public string Name { get; private set; }

        public void Apply(IServer server, IDatabase database)
        {
            try
            {
                _up(server, database);
            }
            catch
            {
                if (_down != null)
                {
                    _down(server, database);
                }
                throw;
            }
        }
    }
}
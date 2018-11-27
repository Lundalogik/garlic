using System;

namespace Garlic
{
    public class GarlicException : Exception
    {
        public GarlicException()
        {
        }

        public GarlicException(string message)
            : base(message)
        {
        }

        public GarlicException(Exception inner)
            : base("", inner)
        {
        }
    }


    public class RevisionNotFoundException : GarlicException
    {
        private readonly string _name;

        public RevisionNotFoundException(string name)
        {
            _name = name;
        }

        public override string Message
        {
            get
            {
                return string.Format("Revision {0} does not exist", _name);
            }
        }
    }
}

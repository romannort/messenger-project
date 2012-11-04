using System;
using System.Net;

namespace iMessenger
{
    public class User : IComparable
    {

        public String Name { get; set; }
        public IPAddress IP { get; set; }

        public User()
        {
            
        }

        public User(String name )
        {
            Name = name;
        }

        protected bool Equals(User other)
        {
            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return Name;
        }

        int IComparable.CompareTo(object obj)
        {
            return obj == null ? 0 : System.String.Compare(Name, ((User) obj).Name, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return ((User)obj).Name == Name;
        }
    }
}

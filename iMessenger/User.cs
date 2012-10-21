using System;

namespace iMessenger
{
    class User : IComparable
    {
        protected bool Equals(User other)
        {
            return string.Equals(Nick, other.Nick);
        }

        public override int GetHashCode()
        {
            return (Nick != null ? Nick.GetHashCode() : 0);
        }

        public String Nick;
        
        public User(String newNick)
        {
            Nick = newNick;
        }

        public override string ToString()
        {
            return Nick;
        }

        int IComparable.CompareTo(object obj)
        {
            return obj == null ? 0 : Nick.CompareTo(((User) obj).Nick);
        }

        public override bool Equals(object obj)
        {
            return ((User)obj).Nick == Nick;
        }
    }
}

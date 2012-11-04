using System;

namespace iMessenger
{
    class User : IComparable
    {
        public readonly String Nick;
        
        protected bool Equals(User other)
        {
            return string.Equals(Nick, other.Nick);
        }

        /// <summary>
        /// Gets Hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (Nick != null ? Nick.GetHashCode() : 0);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="newNick"></param>
        public User(String newNick)
        {
            Nick = newNick;
        }

        /// <summary>
        /// Convert object to string
        /// </summary>
        /// <returns> Object as String </returns>
        public override string ToString()
        {
            return Nick;
        }

        /// <summary>
        /// Compare two objects
        /// </summary>
        /// <param name="obj"> Object to compare </param>
        /// <returns></returns>
        int IComparable.CompareTo(object obj)
        {
            return obj == null ? 0 : String.Compare(Nick, ((User) obj).Nick, StringComparison.Ordinal);
        }

        /// <summary>
        /// Compare two objects
        /// </summary>
        /// <param name="obj"> Object to compare </param>
        /// <returns> Return true if objects are equal </returns>
        public override bool Equals(object obj)
        {
            return ((User)obj).Nick == Nick;
        }
    }
}

using System;
using System.Net;

namespace iMessenger
{
    /// <summary>
    /// Encapsualtes data about user.
    /// </summary>
    public class User : IComparable
    {
        /// <summary>
        /// Displayed user name
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Current user IP address
        /// </summary>
        public IPAddress IP { get; set; }

        public User()
        {
            
        }

        /// <summary>
        /// Initialize constructor
        /// </summary>
        /// <param name="name">New User name</param>
        public User(String name )
        {
            Name = name;
        }

        protected bool Equals(User other)
        {
            return string.Equals(Name, other.Name);
        }

        /// <summary>
        /// Hash code override
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        /// <summary>
        /// Convert object to string
        /// </summary>
        /// <returns> Object as String </returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Compare two objects
        /// </summary>
        /// <param name="obj"> Object to compare </param>
        /// <returns></returns>
        int IComparable.CompareTo(object obj)
        {
            return obj == null ? 0 : String.Compare( Name, ((User) obj).Name, StringComparison.Ordinal);
        }

        /// <summary>
        /// Compare two objects
        /// </summary>
        /// <param name="obj"> Object to compare </param>
        /// <returns> Return true if objects are equal </returns>
        public override bool Equals(object obj)
        {
            return ((User)obj).Name == Name;
        }
    }
}

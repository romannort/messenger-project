using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace iMessenger
{
    public class Roster
    {
        List<Contact> ConList;
        
        public int Count() { return ConList.Count; }        
        public Contact At(int i) { return ConList[i]; }
        public Roster() { ConList = new List<Contact>(); }
        public void Add(IPAddress anIp, String aName, bool isMuted) { ConList.Add(new Contact(anIp, aName, isMuted)); }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace iMessenger
{
    public class Contact
    {
        public IPAddress ip;
        public String name;
        public bool muted;

        public Contact(IPAddress anIp, String aName, bool isMuted)
        {
            ip = anIp;
            name = aName;
            muted = isMuted;
        }
    }
}

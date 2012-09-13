using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace iMessenger
{
    public static class LogHelper
    {
        static String logFile = "messages.log";

        public static void WriteLog(Message m)
        {
            File.AppendAllText(logFile, m.getMessageString() + "\n");
        }
    }
}

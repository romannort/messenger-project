using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace iMessenger
{
    public static class LogHelper
    {
        const String logFile = "messages.log";

        public static void WriteLog(Message m)
        {
            while(true)
                try
                {
                    File.AppendAllText(logFile, m.getMessageString() + "\n", Encoding.Default);
                    return;
                }
            catch(ExecutionEngineException e)
                {

                }
        }
        
    }
}

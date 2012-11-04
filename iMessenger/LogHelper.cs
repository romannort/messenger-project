using System;
using System.Configuration;
using System.Text;
using System.IO;

namespace iMessenger
{
    public static class LogHelper
    {
        
        /// <summary>
        /// Writes message to log
        /// </summary>
        /// <param name="m"> Message to write </param>
        public static void WriteLog(Message m)
        {
            String logFile = ConfigurationManager.AppSettings.Get("logFile");
            while(true)
            { 
                try
                {
                    File.AppendAllText(logFile, m.GetMessageString() + "\n", Encoding.Default);
                    return;
                }
                catch(IOException e)
                {

                }
            }
        }
        
    }
}

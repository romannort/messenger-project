using System;
using System.Text;
using System.IO;

namespace iMessenger
{
    public static class LogHelper
    {
        const String LogFile = "messages.log";

        /// <summary>
        /// Writes message to log
        /// </summary>
        /// <param name="m"> Message to write </param>
        public static void WriteLog(Message m)
        {
            while(true)
            { 
                try
                {
                    File.AppendAllText(LogFile, m.GetMessageString() + "\n", Encoding.Default);
                    return;
                }
                catch(IOException e)
                {

                }
            }
        }
        
    }
}

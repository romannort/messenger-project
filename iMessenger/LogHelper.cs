using System;
using System.Configuration;
using System.IO;

namespace iMessenger
{
    /// <summary>
    /// Helper class for writing logs.
    /// </summary>
    public sealed class LogHelper
    {
        private static volatile LogHelper current; 
        private static Object lockObject = new Object();

        /// <summary>
        /// Property to access helper instance
        /// </summary>
        public static LogHelper Current
        {
            get
            {
                if (current == null)
                {
                    lock (lockObject)
                    {
                        if (current == null)
                        {
                            current = new LogHelper();
                        }
                    }
                }
                return current;
            }
        }
     
        private LogHelper()
        {
            
        }

        /// <summary>
        /// Writes message to log
        /// </summary>
        /// <param name="m"> Message to write </param>
        public void WriteLog(Message m)
        {            
            String logDirectory = ConfigurationManager.AppSettings.Get("LogsDirectory");
            CreateDirectory(logDirectory);
            String currentYear = DateTime.Now.ToString("yyyy");
            logDirectory = Path.Combine(logDirectory, currentYear);
            CreateDirectory(logDirectory);
            String currentMonth = DateTime.Now.ToString("MMMM");
            logDirectory = Path.Combine(logDirectory, currentMonth);
            CreateDirectory( logDirectory );
            String fileName = Path.Combine(logDirectory,
                                           (m.Type == MessageType.Common ? "Common" : m.ConferenceNumber) + ".log");
            CreateFile( fileName );
            using (FileStream fs = File.OpenWrite(fileName))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(m.GetMessageString());
                    writer.Close();
                }    
            }            
        }

        /// <summary>
        /// Creates directory if it no exist.
        /// </summary>
        /// <param name="path">Path to directory</param>
        private static void CreateDirectory(String path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }            
        }

        /// <summary>
        /// Creates file if it isn't exist.
        /// </summary>
        /// <param name="path"></param>
        private static void CreateFile(String path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }
        
    }
}

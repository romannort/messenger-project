using System;
using System.Configuration;
using System.Text;
using System.IO;

namespace iMessenger
{
    /// <summary>
    /// Helper class for writing logs.
    /// </summary>
    public sealed class LogHelper
    {
        
        private static readonly LogHelper current = new LogHelper();

        /// <summary>
        /// Property to access helper instance
        /// </summary>
        public static LogHelper Current
        {
            get { return current; }
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
            while(true)
            { 
                try
                {
                    File.AppendAllText(fileName, m.GetMessageString() + "\n", Encoding.Default);
                    break;
                }
                catch(IOException e)
                {
                    
                }
            }
        }

        /// <summary>
        /// Creates directory if it no exist.
        /// </summary>
        /// <param name="path">Path to directory</param>
        private void CreateDirectory(String path)
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
        private void CreateFile(String path)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
            }
        }
        
    }
}

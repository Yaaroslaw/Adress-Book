using System;
using System.IO;

namespace Logger
{
    //Interface to describe Logger class for using in Strategy pattern
    public interface ILogger
    {
        void Info   (string message);
        void Debug  (string message);
        void Warning(string message);
        void Error  (string message);
    }

    //Logger for writing in file
    public class FileLogger : ILogger
    {
        public void Info(string message)
        {
            using (StreamWriter file = new StreamWriter("log.txt", true))
            {
                file.WriteLine("INFO: {0}", message);
            }
        }

        public void Debug(string message)
        {
            using (StreamWriter file = new StreamWriter("log.txt", true))
            {
                file.WriteLine("DEBUG: {0}", message);
            }
        }

        public void Warning(string message)
        {
            using (StreamWriter file = new StreamWriter("log.txt", true))
            {
                file.WriteLine("WARNING: {0}", message);
            }
        }

        public void Error(string message)
        {
            using (StreamWriter file = new StreamWriter("log.txt", true))
            {
                file.WriteLine("ERROR: {0}", message);
            }
        }
    }

    //Logger for writing in console
    public class ConsoleLogger : ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine("INFO: {0}", message);
        }

        public void Debug(string message)
        {
            Console.WriteLine("DEBUG: {0}", message);
        }

        public void Warning(string message)
        {
            Console.WriteLine("WARNING: {0}", message);
        }

        public void Error(string message)
        {
            Console.WriteLine("ERROR: {0}", message);
        }
    }

    //Logger switching between ConsoleLogger and FileLogger
    public class Logger
    {
        private ILogger _logger;

        public Logger(ILogger logger)
        {
            if (logger == null)
                throw new NullReferenceException("Logger was not implemented");
            _logger = logger;
        }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Warning(string message)
        {
            _logger.Warning(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }
    }
}

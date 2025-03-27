using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxCode.Model
{
    public class NLogModel
    {
        public string Message { get; set; }
        public string LogLevel { get; set; }
        public Exception Exception { get; set; }
        public DateTime Timestamp { get; set; }

        public NLogModel(string message, string logLevel, Exception exception = null)
        {
            Message = message;
            LogLevel = logLevel;
            Exception = exception;
            Timestamp = DateTime.Now;
        }
    }
}

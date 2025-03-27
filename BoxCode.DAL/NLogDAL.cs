using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxCode.Model;
using NLog;

namespace BoxCode.DAL
{
    public class NLogDAL
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private NLogDAL() { }

        private static readonly Lazy<NLogDAL> instance = new Lazy<NLogDAL>(() => new NLogDAL());

        public static NLogDAL Instance => instance.Value;

        public void LogInfo(NLogModel logModel)
        {
            logger.Info($"{logModel.Timestamp} [{logModel.LogLevel}] {logModel.Message}");
        }

        public void LogWarning(NLogModel logModel)
        {
            logger.Warn($"{logModel.Timestamp} [{logModel.LogLevel}] {logModel.Message}");
        }

        public void LogError(NLogModel logModel)
        {
            if (logModel.Exception != null)
            {
                logger.Error(logModel.Exception, $"{logModel.Timestamp} [{logModel.LogLevel}] {logModel.Message}");
            }
            else
            {
                logger.Error($"{logModel.Timestamp} [{logModel.LogLevel}] {logModel.Message}");
            }
        }
    }
}

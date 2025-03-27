using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using NLog.Targets;
using NLog.Config;
using System.IO;

namespace BoxCode
{
    internal static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {

            // 設定 NLog 配置
            var config = new LoggingConfiguration();
            // 取得當前程式的資料夾路徑
            var currentDirectory = Directory.GetCurrentDirectory();

            var debugDirectory = Path.Combine(currentDirectory, "debug");
            if (!Directory.Exists(debugDirectory))
            {
                Directory.CreateDirectory(debugDirectory);
            }
            // 創建日誌輸出到文件的目標
            // 設定日誌儲存到 "debug" 資料夾下
            var logfile = new FileTarget("logfile")
            {
                FileName = Path.Combine(currentDirectory, "debug", "log-${shortdate}.txt") // 動態設置日誌路徑
            };

            // 設定日誌輸出格式
            logfile.Layout = "${longdate} ${level:uppercase=true} ${message} ${exception}";

            // 將目標加到配置中
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);

            // 設定配置
            LogManager.Configuration = config;

            // 取得記錄器
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Start Program..");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmLogin());
        }
    }
}

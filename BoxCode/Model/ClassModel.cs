using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BoxCode.Model
{
    public class ClassModel
    {
        public static string logPath = "C:\\ASMP\\log";

        public static string logWorkOrder = "";

        public static string logWorkStage = "";

        public static string logEmployeeNumber = "";

        public static string logSN = "000000";

        public static void WriteLog(string logMsg)
        {
            /*string text = DateTime.Now.Year + int.Parse(DateTime.Now.Month.ToString()).ToString("00") + int.Parse(DateTime.Now.Day.ToString()).ToString("00") + ".txt";
            string text2 = int.Parse(DateTime.Now.Hour.ToString()).ToString("00") + ":" + int.Parse(DateTime.Now.Minute.ToString()).ToString("00") + ":" + int.Parse(DateTime.Now.Second.ToString()).ToString("00");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            if (!File.Exists(logPath + "\\" + text))
            {
                File.Create(logPath + "\\" + text).Close();
            }

            string value = "WorkOrder:" + logWorkOrder + ",Serial-Number:" + logSN + ",WorkStage:" + logWorkStage + ",EmployeeNumber:" + logEmployeeNumber + ",Time:" + text2 + ",Result:" + logMsg;
            using StreamWriter streamWriter = File.AppendText(logPath + "\\" + text);
            streamWriter.Write(value);
            streamWriter.WriteLine("");*/
        }
    }

}

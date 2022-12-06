using System;
using log4net;
using log4net.Config;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace LogApp
{
    public class Log4Net
    {

        private static bool enableDBLOG = false;
        private static bool enableGENERALLOG = false;
        private static bool enableERRORLOG = false;
        private static bool enableCOMMSLOG = false;
        private static bool enableTELEPHONYLOG = false;
        private static bool enableSECURELOG = false;
        private static string fileName = string.Empty;
        private static string filePath = string.Empty;
        private static int fileSize = 1024;
        private static int totalFiles = 5;

        #region Get/Set Methods

        public static int FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }

        public static int TotalFiles
        {
            get { return totalFiles; }
            set { totalFiles = value; }
        }

        public static bool EnableCOMMSLOG
        {
            get { return enableCOMMSLOG; }
            set { enableCOMMSLOG = value; }
        }

        public static bool EnableSECURELOG
        {
            get { return enableSECURELOG; }
            set { enableSECURELOG = value; }
        }

        public static bool EnableTELEPHONYLOG
        {
            get { return enableTELEPHONYLOG; }
            set { enableTELEPHONYLOG = value; }
        }

        public static bool EnableDBLOG
        {
            get { return enableDBLOG; }
            set { enableDBLOG = value; }
        }

        public static bool EnableGENERALLOG
        {
            get { return enableGENERALLOG; }
            set { enableGENERALLOG = value; }
        }

        public static bool EnableERRORLOG
        {
            get { return enableERRORLOG; }
            set { enableERRORLOG = value; }
        }

        public static string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public static string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        #endregion Get/Set Methods

        public static log4net.ILog logger = log4net.LogManager.GetLogger("LogApp");

        private static StringBuilder GetMethodAndClassName()
        {
            StringBuilder preamble = null;
            string[] strClassAndMethod = new string[2];
            try
            {

                preamble = new StringBuilder();

                StackTrace stackTrace = new StackTrace();
                StackFrame stackFrame;
                MethodBase stackFrameMethod;

                int frameCount = 0;
                string typeName;
                do
                {
                    frameCount++;
                    stackFrame = stackTrace.GetFrame(frameCount);
                    stackFrameMethod = stackFrame.GetMethod();
                    typeName = stackFrameMethod.ReflectedType.FullName;
                } while (typeName.StartsWith("System") || typeName.EndsWith("LogApp.Log4Net"));

                string strDel = ".";
                char[] CharArrDelim = strDel.ToCharArray();
                string[] strBroken = typeName.Split(CharArrDelim, typeName.Length);


                preamble.Append(stackFrameMethod.Name);
            }
            catch (Exception E)
            {
                string str = E.Message;
            }
            return preamble;
        }

        public static void Activate(bool IsWeb)
        {
            try
            {
                logger = log4net.LogManager.GetLogger("LogApp");
                string logFileTo = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
                if (IsWeb)
                {
                    logFileTo = System.Web.HttpContext.Current.Server.MapPath(".") + @"\bin";
                }
                log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(logFileTo + "\\LogApp.dll.config"));
                //log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(@"D:\Projects\Votel\Camba\camba\WebUI\Bin\LogApp.dll.config"));
                createLogPathDirectory(filePath);
                foreach (log4net.Appender.IAppender appender in log4net.LogManager.GetRepository().GetAppenders())
                {
                    if (appender is log4net.Appender.RollingFileAppender)
                    {
                        log4net.Appender.RollingFileAppender rollingFileAppender = (log4net.Appender.RollingFileAppender)appender;
                        rollingFileAppender.File = filePath + "\\" + fileName;
                        rollingFileAppender.MaximumFileSize = Convert.ToString(fileSize) + "MB";
                        rollingFileAppender.MaxSizeRollBackups = totalFiles;
                        rollingFileAppender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size;
                        rollingFileAppender.ActivateOptions();
                    }
                }
            }
            catch (Exception E)
            {
                string str = E.Message;
            }
        }

        public static bool WriteLog(string msg, LogType LLevel)
        {
            try
            {
                switch (LLevel)
                {
                    case LogType.DBLOG:
                        msg = "[" + GetMethodAndClassName() + "] " + msg;
                        logger.Info(msg);
                        break;
                    case LogType.ERRORLOG:
                        msg = "[" + GetMethodAndClassName() + "] " + msg;
                        logger.Info(msg);
                        break;
                    case LogType.GENERALLOG:
                        msg = "[" + GetMethodAndClassName() + "] " + msg;
                        logger.Info(msg);
                        break;
                    case LogType.COMMSLOG:
                        msg = "[" + GetMethodAndClassName() + "] " + msg;
                        logger.Info(msg);
                        break;
                    case LogType.SECURELOG:
                        msg = "[" + GetMethodAndClassName() + "] " + msg;
                        logger.Info(msg);
                        break;
                    case LogType.TELEPHONYLOG:
                        msg = "[" + GetMethodAndClassName() + "] " + msg;
                        logger.Info(msg);
                        break;
                }
            }
            catch (Exception ex)
            {
                //logger.TELEPHONY_ERROR(ex.Message, ex, false);
            }
            return true;
        }

        public static bool WriteException(Exception E)
        {
            try
            {
                logger.Error("[" + GetMethodAndClassName() + "] " + E.Message, E);
            }
            catch (Exception ex)
            {
                //logger.TELEPHONY_ERROR(ex.Message, ex, false);
            }
            return true;
        }

        private static void createLogPathDirectory(string filePath)
        {
            try
            {               
                if (Directory.Exists(filePath) == false)
                {
                    Directory.CreateDirectory(filePath);
                }

            }
            catch (Exception E)
            {

            }
        }

    } // end class name

    public enum LogType
    {
        COMMSLOG,
        DBLOG,
        GENERALLOG,
        TELEPHONYLOG,
        ERRORLOG,
        SECURELOG
    }
}
// end name space

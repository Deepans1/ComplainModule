namespace ComplainModule.Log
{
    public class QtXLogger
    {
        #region " Variables "

        private readonly IWebHostEnvironment _webHostEnvironment;
        private static string logFileName = "QTXLog";
        string LogFilePath = String.Empty;

        #endregion

        #region " Initlize the Folder Path  "
        public QtXLogger(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        #region " Create a Log File "

        public void Log(string strmsg)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            
            LogFilePath = Path.Combine(webRootPath, "Logs");
            LogFilePath = LogFilePath + "\\QtxLog";

            if (!Directory.Exists(LogFilePath))
                Directory.CreateDirectory(LogFilePath);

            string LogFile = logFileName + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            if (!File.Exists(LogFile))
                File.Create(LogFile);

            LogFilePath = LogFilePath + "\\" + LogFile;

            lock (new object())
            {
                using (var stream = File.Open(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    StreamWriter sw = new StreamWriter(stream);
                    sw.WriteLine("[" + DateTime.Now.ToString() + "] : " + strmsg);
                    sw.Close();
                    stream.Close();
                }
            }
        }

        #endregion
    }
}
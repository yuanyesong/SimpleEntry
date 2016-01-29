﻿using System;
using System.IO;
using System.Reflection;

namespace SimpleEntry.Services
{
    class LogWritter
    {
        /// <summary>
        /// 错误日志记录类
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="title"></param>
        /// <param name="aditional"></param>
        public static void Log(Exception ex, string title, object aditional = null)
        {
            #region Creates the Folder if not Exists

            if (!Directory.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Logs\\"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Logs\\");
            }

            #endregion

            #region Creates the File

            string dateAppendage = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year;

            FileStream fs = null;

            try
            {
                //Check the file, create it if necessary.
                //Writes it using another FileStream.
                fs = new FileStream(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Logs\\log_" + dateAppendage + ".txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            catch (Exception)
            {
                //File may be in use.
                dateAppendage = DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day;
                fs = new FileStream(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Logs\\log_" + dateAppendage + ".txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }

            var s = new StreamWriter(fs);
            s.Close();
            fs.Close();
            fs.Dispose();

            #endregion

            var fs1 = new FileStream(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Logs\\log_" + dateAppendage + ".txt", FileMode.Append, FileAccess.Write);

            var s1 = new StreamWriter(fs1);

            s1.WriteLine("► Title - {0}\t{1}", Environment.NewLine, title);
            s1.WriteLine("▬ Message - {0}\t{1}", Environment.NewLine, ex.Message);
            s1.WriteLine("♦ Date/Hour - {0}\t{1}", Environment.NewLine, DateTime.Now);
            s1.WriteLine("▲ Source - {0}\t{1}", Environment.NewLine, ex.Source);
            s1.WriteLine("▼ TargetSite - {0}\t{1}", Environment.NewLine, ex.TargetSite);
            if (aditional != null)
                s1.WriteLine("◄ Aditional - {0}\t{1}", Environment.NewLine, aditional);
            s1.WriteLine("♠ StackTrace - {0}{1}", Environment.NewLine, ex.StackTrace);

            if (ex.InnerException != null)
            {
                s1.WriteLine();
                s1.WriteLine("▬▬ Message - {0}\t{1}", Environment.NewLine, ex.InnerException.Message);
                s1.WriteLine("▲▲ Source - {0}\t{1}", Environment.NewLine, ex.InnerException.Source);
                s1.WriteLine("▼▼ TargetSite - {0}\t{1}", Environment.NewLine, ex.InnerException.TargetSite);
                s1.WriteLine("♠♠ StackTrace - {0}{1}", Environment.NewLine, ex.InnerException.StackTrace);

                if (ex.InnerException.InnerException != null)
                {
                    s1.WriteLine();
                    s1.WriteLine("▬▬▬ Message - {0}\t{1}", Environment.NewLine, ex.InnerException.InnerException.Message);
                    s1.WriteLine("▲▲▲ Source - {0}\t{1}", Environment.NewLine, ex.InnerException.InnerException.Source);
                    s1.WriteLine("▼▼▼ TargetSite - {0}\t{1}", Environment.NewLine, ex.InnerException.InnerException.TargetSite);
                    s1.WriteLine("♠♠♠ StackTrace - {0}\t{1}", Environment.NewLine, ex.InnerException.InnerException.StackTrace);
                }
            }

            s1.WriteLine();
            s1.WriteLine("----------------------------------");
            s1.WriteLine();

            s1.Close();
            fs1.Close();
            fs1.Dispose();
        }
    }
}

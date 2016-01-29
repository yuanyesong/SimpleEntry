using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntry.Services
{
    class GetFileNameOrFilePath
    {
        #region 获取文件路径和文件名
        /// <summary>
        /// 获取文件路径方法
        /// </summary>
        /// <param name="fileName">带文件名的路径</param>
        /// <returns>文件路径</returns>
        public static string GetPath(string fileName)
        {
            string path = fileName.Substring(0, fileName.LastIndexOf("\\"));
            return path;
        }

        /// <summary>
        /// 获取纯文件名
        /// </summary>
        /// <param name="fileName">带文件名的路径</param>
        /// <returns>纯文件名</returns>
        public static string GetFileName(string fileName)
        {
            string onlyFileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
            return onlyFileName;
        }
        #endregion
    }
}

using System;
using System.Data;
using System.Data.SQLite;

namespace SimpleEntry.Services
{
    class ValidateDataBase
    {
        /// <summary>
        /// 验证lib文件是不是一个有效的库文件
        /// </summary>
        /// <param name="DataSource"></param>
        /// <returns></returns>
        public static bool Validate(string DataSource)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    try
                    {
                        DataTable dt = sh.Select("select * from QuestionInfo");
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
        }
    }
}

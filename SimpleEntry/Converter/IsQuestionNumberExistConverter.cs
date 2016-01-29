using System;
using System.Data.SQLite;
using System.Windows.Data;

namespace SimpleEntry.Converter
{
    class IsQuestionNumberExistConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            using (SQLiteConnection conn = new SQLiteConnection("StaticConfig.DataSource"))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    int countCurrent = 0;
                    string sqlCommandCurrent = String.Format("select count(*) from QuestionInfo where Q_Num={0};", value);//判断当前题号是否存在
                    try
                    {
                        Int32.Parse((string)value);
                        countCurrent = sh.ExecuteScalar<int>(sqlCommandCurrent);
                        conn.Close();
                        if (countCurrent > 0)
                        {
                            return "更新";
                        }
                        else
                        {
                            return "保存";
                        }
                    }
                    catch (Exception)
                    {
                        return "保存";
                    }
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}

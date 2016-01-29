using SimpleEntry.Models;
using System;
using System.Data;
using System.Data.SQLite;

namespace SimpleEntry.Services
{
    /// <summary>
    /// 通过Sqlite数据库实现IDataServices
    /// </summary>
    class SqliteDataServices : IDataServices
    {

        public QuestionInfo GetAllQuestionInfo(int questionNumber, string DataSource)
        {
            //Config.DataBaseFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "MyData.db");
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    DataTable dt = sh.Select("select * from QuestionInfo where Q_Num=@questionNumber;", new SQLiteParameter[] { new SQLiteParameter("questionNumber", questionNumber) });
                    foreach (DataRow dr in dt.Rows)
                    {
                        int otherOption;
                        int musterEnter;
                        int repeat;
                        int jump;
                        QuestionInfo questionInfo = new QuestionInfo();
                        questionInfo.QuestionNumber = dr["Q_Num"].ToString();
                        questionInfo.QuestionTypeID = Convert.ToInt32(dr["TypeID"]);
                        questionInfo.QuestionContent = dr["Q_Content"].ToString();
                        questionInfo.QuestionField = dr["Q_Field"].ToString();
                        questionInfo.QuestionLable = dr["Q_Lable"].ToString();
                        questionInfo.OptionsCount = dr["Q_OptionsCount"].ToString();
                        otherOption = Convert.ToInt32(dr["Q_OtherOption"]);
                        if (otherOption==0)
                        {
                            questionInfo.OtherOption = false;
                        }
                        else
                        {
                            questionInfo.OtherOption = true;
                        }
                        questionInfo.ValueLable = dr["Q_ValueLable"].ToString();
                        questionInfo.DataTypeID = Convert.ToInt32(dr["DataTypeID"]);
                        questionInfo.ValueRange = dr["Q_ValueRange"].ToString();
                        questionInfo.Pattern = dr["Q_Pattern"].ToString();
                        musterEnter = Convert.ToInt32(dr["Q_MustEnter"]);
                        if (musterEnter == 0)
                        {
                            questionInfo.IsMustEnter = false;
                        }
                        else
                        {
                            questionInfo.IsMustEnter = true;
                        }
                        repeat = Convert.ToInt32(dr["Q_Repeat"]);
                        if (repeat == 0)
                        {
                            questionInfo.IsRepeat = false;
                        }
                        else
                        {
                            questionInfo.IsRepeat = true;
                        }
                        jump = Convert.ToInt32(dr["Q_Jump"]);
                        if (jump == 0)
                        {
                            questionInfo.IsJump = false;
                        }
                        else
                        {
                            questionInfo.IsJump = true;
                        }
                        questionInfo.JumpConditions = dr["Q_JumpConditions"].ToString();
                        questionInfo.JumpTarget = dr["Q_JumpTarget"].ToString();
                        questionInfo.DataBaseFile = DataSource.Substring(12);
                        //if (DataSource == string.Format("data source={0}", Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "MyData.db")))
                        //{
                            int count = 0;
                            string sqlString = String.Format("select count(*) from QuestionInfo where Q_Field='{0}' COLLATE NOCASE", questionInfo.QuestionField.ToLower());//判断当前字段是否存在,注意字段值一定要加单引号
                            count = sh.ExecuteScalar<int>(sqlString);
                            if (count==1)
                            {
                                IsQuestionFieldExist.Instance.IsExist = true;
                            }
                            else
                            {
                                IsQuestionFieldExist.Instance.IsExist = false;
                            }
                        //}
                        return questionInfo;
                    }
                    conn.Close();
                    return null;
                }
            }
        }
    }
}

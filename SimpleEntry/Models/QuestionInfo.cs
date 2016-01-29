using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleEntry.Models
{
    /// <summary>
    /// QuestionInfo实体Model类
    /// </summary>
    public class QuestionInfo : BindableBase, IDataErrorInfo
    {
        /// <summary>
        /// 题号
        /// </summary>
        #region QuestionNumber
        private string questionNumber;
        public string QuestionNumber
        {
            get { return questionNumber; }
            set
            {
                SetProperty(ref questionNumber, value.Replace(" ", ""));
            }

        }
        #endregion

        /// <summary>
        /// 题型
        /// </summary>
        #region QuestionTypeID
        private int questionTypeID;
        public int QuestionTypeID
        {
            get { return questionTypeID; }
            set
            {
                SetProperty(ref questionTypeID, value);
                OnPropertyChanged("QuestionLable");
                OnPropertyChanged("OptionsCount");
                OnPropertyChanged("ValueLable");
                OnPropertyChanged("DataTypeID");
                OnPropertyChanged("ValueRange");
                OnPropertyChanged("Pattern");
                OnPropertyChanged("JumpConditions");
            }
        }
        #endregion

        /// <summary>
        /// 题目内容
        /// </summary>
        #region QuestionContent
        private string questionContent;
        public string QuestionContent
        {
            get { return questionContent; }
            set { SetProperty(ref questionContent, value); }
        }
        #endregion

        /// <summary>
        /// 字段
        /// </summary>
        #region QuestionField
        private string questionField;
        public string QuestionField
        {
            get { return questionField; }
            set { SetProperty(ref questionField, value.Replace(" ", "")); }
        }
        #endregion

        /// <summary>
        /// 标签
        /// </summary>
        #region QuestionLable
        private string questionLable;
        public string QuestionLable
        {
            get { return questionLable; }
            set { SetProperty(ref questionLable, value); }
        }
        #endregion

        /// <summary>
        /// 选项个数
        /// </summary>
        #region OptionsCount
        private string optionsCount;
        public string OptionsCount
        {
            get { return optionsCount; }
            set
            {
                SetProperty(ref optionsCount, value.Replace(" ", ""));
                OnPropertyChanged("QuestionLable");
                OnPropertyChanged("ValueLable");
            }
        }
        #endregion

        /// <summary>
        /// 其他选项
        /// </summary>
        #region OtherOption
        private bool otherOption;
        public bool OtherOption
        {
            get { return otherOption; }
            set
            {
                SetProperty(ref otherOption, value);
                OnPropertyChanged("OptionsCount");
            }
        }
        #endregion

        /// <summary>
        /// 值标签
        /// </summary>
        #region ValueLable
        private string valueLable;
        public string ValueLable
        {
            get { return valueLable; }
            set { SetProperty(ref valueLable, value); }
        }
        #endregion

        /// <summary>
        /// 数据类型
        /// </summary>
        #region DataTypeID
        private int dataTypeID;
        public int DataTypeID
        {
            get { return dataTypeID; }
            set
            {
                SetProperty(ref dataTypeID, value);
                OnPropertyChanged("ValueRange");
                OnPropertyChanged("Pattern");
            }
        }
        #endregion

        /// <summary>
        /// 取值范围
        /// </summary>
        #region ValueRange
        private string valueRange;
        public string ValueRange
        {
            get { return valueRange; }
            set { SetProperty(ref valueRange, value.Replace(" ", "")); }
        }
        #endregion

        /// <summary>
        /// 正则表达式
        /// </summary>
        #region Pattern
        private string pattern;
        public string Pattern
        {
            get { return pattern; }
            set { SetProperty(ref pattern, value.Trim()); }
        }
        #endregion

        /// <summary>
        /// 非空
        /// </summary>
        #region IsMustEnter
        private bool isMustEnter;
        public bool IsMustEnter
        {
            get { return isMustEnter; }
            set { SetProperty(ref isMustEnter, value); }
        }
        #endregion

        /// <summary>
        /// 重复
        /// </summary>
        #region IsRepeat
        private bool isRepeat;
        public bool IsRepeat
        {
            get { return isRepeat; }
            set { SetProperty(ref isRepeat, value); }
        }
        #endregion

        /// <summary>
        /// 跳转
        /// </summary>
        #region IsJump
        private bool isJump;
        public bool IsJump
        {
            get { return isJump; }
            set
            {
                SetProperty(ref isJump, value);
                //OnPropertyChanged("jumpConditions");
                //OnPropertyChanged("JumpTarget");
            }
        }
        #endregion

        /// <summary>
        /// 跳转条件
        /// </summary>
        #region JumpConditions
        private string jumpConditions;
        public string JumpConditions
        {
            get { return jumpConditions; }
            set { SetProperty(ref jumpConditions, value.Replace(" ", "")); }
        }
        #endregion

        /// <summary>
        /// 跳转到
        /// </summary>
        #region JumpTarget
        private string jumpTarget;
        public string JumpTarget
        {
            get { return jumpTarget; }
            set { SetProperty(ref jumpTarget, value.Replace(" ", "")); }
        }
        #endregion

        /// <summary>
        /// 数据库路径
        /// </summary>
        #region DataBasePath
        public string DataBaseFile;
        private string DataSource
        {
            get
            {
                return string.Format("data source={0}", DataBaseFile);
            }
        }
        #endregion

        #region 属性验证
        #region IDataErrorInfo Members
        public string Error
        {
            get { return null; }
        }

        public string this[string propertyName]
        {
            get { return IsValid(propertyName); }
        }
        #endregion

        /// <summary>
        /// 验证所有输入是否合法
        /// </summary>
        /// <returns>All Property IsValid</returns>
        public bool IsValid()
        {
            return string.IsNullOrEmpty(IsValid("QuestionContent")) &&
                string.IsNullOrEmpty(IsValid("QuestionNumber")) &&
                string.IsNullOrEmpty(IsValid("QuestionTypeID")) &&
                string.IsNullOrEmpty(IsValid("QuestionField")) &&
                string.IsNullOrEmpty(IsValid("QuestionLable")) &&
                string.IsNullOrEmpty(IsValid("OptionsCount")) &&
                string.IsNullOrEmpty(IsValid("ValueLable")) &&
                string.IsNullOrEmpty(IsValid("DataTypeID")) &&
                string.IsNullOrEmpty(IsValid("JumpTarget")) &&
                string.IsNullOrEmpty(IsValid("JumpConditions")) &&
                string.IsNullOrEmpty(IsValid("ValueRange")) &&
                string.IsNullOrEmpty(IsValid("Pattern"));
        }

        /// <summary>
        /// 验证单个属性输入是否合法
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>Single Property IsValid</returns>
        private string IsValid(string propertyName)
        {
            switch (propertyName)
            {
                case "QuestionContent":
                    if (string.IsNullOrWhiteSpace(QuestionContent))
                    {
                        return "内容不能为空";
                    }
                    break;
                case "QuestionNumber":
                    if (string.IsNullOrWhiteSpace(QuestionNumber) || !isValidNumber(QuestionNumber))
                    {
                        return "只能输入大于0的数字";
                    }
                    break;
                case "QuestionTypeID":
                    if (QuestionTypeID == -1)
                    {
                        return "请选择";
                    }
                    break;
                case "QuestionField":
                    {
                        if (!string.IsNullOrEmpty(QuestionField))
                        {
                            if (Regex.IsMatch(QuestionField.Substring(0, 1), @"^[\u4e00-\u9fa5a-zA-Z]") && !QuestionField.EndsWith(".") && !QuestionField.EndsWith("_"))
                            {
                                char[] variableName = QuestionField.ToCharArray();
                                if (!variableName.Contains('!') && !variableName.Contains('?') && !variableName.Contains('*'))
                                {
                                    string[] reservedKeywords = new string[] { "ALL", "NE", "EQ", "TO", "LE", "LT", "BY", "OR", "GT", "AND", "NOT", "GE", "WITH" };
                                    for (int i = 0; i < reservedKeywords.Length; i++)
                                    {
                                        if (string.Compare(QuestionField, reservedKeywords[i], true) != 0)
                                        {
                                            int length = System.Text.Encoding.Default.GetByteCount(QuestionField);
                                            if (length > 65)
                                            {
                                                return "长度不能超过64字节";
                                            }
                                            else
                                            {
                                                //string DataBaseFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "MyData.db");
                                                //string DataSource = string.Format("data source={0}", DataBaseFile);
                                                using (SQLiteConnection conn = new SQLiteConnection(DataSource))
                                                {
                                                    using (SQLiteCommand cmd = new SQLiteCommand())
                                                    {
                                                        cmd.Connection = conn;
                                                        conn.Open();
                                                        SQLiteHelper sh = new SQLiteHelper(cmd);
                                                        int count = 0;
                                                        string sqlString = String.Format("select count(*) from QuestionInfo where Q_Field='{0}' COLLATE NOCASE", QuestionField);//判断当前字段是否存在,注意字段值一定要加单引号
                                                        count = sh.ExecuteScalar<int>(sqlString);
                                                        conn.Close();
                                                        if (count > 0)
                                                        {
                                                            if (count == 1)
                                                            {
                                                                if (!IsQuestionFieldExist.Instance.IsExist)
                                                                {
                                                                    return "字段已存在";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                return "字段已存在";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            return "不能使用以下保留关键字（不区分大小写）：ALL, NE, EQ, TO, LE, LT, BY, OR, GT, AND, NOT, GE, WITH";
                                        }
                                    }
                                }
                                else
                                {
                                    return "不能包含特殊字符（!?*）";
                                }
                            }
                            else
                            {
                                return "只能以字母或汉字开头，不能以“.”或“_”结尾";
                            }
                        }
                    }
                    break;
                case "QuestionLable":
                    {
                        if (QuestionTypeID == 1)
                        {
                            if (!string.IsNullOrEmpty(QuestionLable))
                            {
                                string[] lables = Regex.Split(QuestionLable, "\\s*,\\s*");
                                if (lables.Length.ToString() == OptionsCount)
                                {
                                    foreach (var lable in lables)
                                    {
                                        if (string.IsNullOrWhiteSpace(lable))
                                        {
                                            return "标签个数必须与选项个数相等";
                                        }
                                    }
                                }
                                else
                                {
                                    return "标签个数必须与选项个数相等";
                                }
                            }
                        }
                    }
                    break;
                case "OptionsCount":
                    if ((string.IsNullOrWhiteSpace(OptionsCount) || !isValidNumber(OptionsCount)) && (QuestionTypeID == 0 || QuestionTypeID == 1))
                    {
                        return "只能输入大于0的数字";
                    }
                    break;
                case "ValueLable":
                    {
                        if (!string.IsNullOrEmpty(ValueLable) && QuestionTypeID == 0)
                        {
                            string[] lables = Regex.Split(ValueLable, "\\s*,\\s*");
                            if (lables.Length.ToString() == OptionsCount)
                            {
                                foreach (var lable in lables)
                                {
                                    if (string.IsNullOrWhiteSpace(lable))
                                    {
                                        return "值标签个数必须与选项个数相等";
                                    }
                                }
                            }
                            else
                            {
                                return "值标签个数必须与选项个数相等";
                            }
                        }
                    }
                    break;
                case "DataTypeID":
                    if ((DataTypeID == -1) && QuestionTypeID == 3)
                    {
                        return "请选择";
                    }
                    break;
                case "JumpTarget":
                    if (((string.IsNullOrWhiteSpace(JumpTarget) || !isValidNumber(JumpTarget))) && (IsJump == true))
                    {
                        return "只能输入大于0的数字";
                    }
                    break;
                case "JumpConditions":
                    {
                        if (QuestionTypeID != 3)
                        {
                            if (QuestionTypeID != 2)
                            {
                                string jumpConditionsPattern = @"^\d+(?:-\d+)?(?:,\d+(?:-\d+)?)*$"; //@"^\d+(\d+)?$|^\d+(\,\d+)+(\d+)?$";
                                if (!(Regex.IsMatch(JumpConditions, jumpConditionsPattern)) && (IsJump == true))
                                {
                                    return "只能输入以英文逗号或横杆分隔的数字";
                                }
                            }
                            else
                            {
                                if ((JumpConditions == "对" || JumpConditions == "错"))
                                {

                                }
                                else if (IsJump == true)
                                {
                                    return "只能输入“对”或“错”";
                                }
                            }
                        }
                    }
                    break;
                case "ValueRange":
                    {
                        if (!string.IsNullOrEmpty(valueRange) && QuestionTypeID == 3 && DataTypeID != 1)
                        {
                            if (DataTypeID == 0)
                            {
                                string valueRangePattern = @"^-?\d+(\.\d+)?(?:--?\d+(\.\d+)?)?(?:,-?\d+(\.\d+)?(?:--?\d+(\.\d+)?)?)*$";
                                if (!Regex.IsMatch(ValueRange, valueRangePattern) && !string.IsNullOrEmpty(ValueRange))
                                {
                                    return "逗号匹配单个值，横杠匹配区间，例如：1,2,3-10";
                                }
                            }
                            if (DataTypeID == 2)
                            {
                                string datePattern = @"^(?:(?!0000)[0-9]{4}/(?:(?:0[1-9]|1[0-2])/(?:0[1-9]|1[0-9]|2[0-8])|(?:0[13-9]|1[0-2])/(?:29|30)|(?:0[13578]|1[02])/31)|(?:[0-9]{2}(?:0[48]|[2468][048]|[13579][26])|(?:0[48]|[2468][048]|[13579][26])00)/02/29)$";
                                string errorMessage = "按以下格式输入：2000/02/29-2001/02/28";
                                Dictionary<char, int> dic = new Dictionary<char, int>();
                                char[] charData = ValueRange.ToCharArray();
                                foreach (char c in charData)
                                {
                                    if (!dic.ContainsKey(c))
                                    {
                                        dic.Add(c, 1);
                                    }
                                    else
                                    {
                                        dic[c] = dic[c] + 1;
                                    }
                                }
                                if (!string.IsNullOrEmpty(ValueRange))
                                {
                                    if (dic.ContainsKey('-'))
                                    {
                                        if (dic['-'] == 1)
                                        {
                                            int count = 0;
                                            string[] dates = valueRange.Split('-');
                                            foreach (var item in dates)
                                            {
                                                if (item != "")
                                                {
                                                    if (Regex.IsMatch(item, datePattern))
                                                    {

                                                    }
                                                    else
                                                    {
                                                        return errorMessage;
                                                    }
                                                }
                                                else
                                                {
                                                    count++;
                                                }
                                            }
                                            if (count == 2)
                                            {
                                                return errorMessage;
                                            }
                                            else
                                            {

                                            }
                                        }
                                        else
                                        {
                                            return errorMessage;
                                        }
                                    }
                                    else
                                    {
                                        return errorMessage;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "Pattern":
                    {
                        if ((Pattern != null) && (Pattern.Trim().Length > 0))
                        {
                            try
                            {
                                Regex.Match("", Pattern);
                            }
                            catch (ArgumentException)
                            {
                                // BAD PATTERN: Syntax error
                                return "无效的正则表达式";
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// 验证数字是否有效
        /// </summary>
        /// <param name="value"></param>
        /// <returns>isValidNumber</returns>
        private bool isValidNumber(string value)
        {
            int _value;
            try
            {
                _value = Int32.Parse(value);
            }
            catch (Exception)
            {
                return false;
            }
            if (_value <= 0)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}

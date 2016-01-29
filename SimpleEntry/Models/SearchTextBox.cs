using Microsoft.Practices.Prism.Mvvm;
using System;
using System.ComponentModel;

namespace SimpleEntry.Models
{
    /// <summary>
    /// 查询文本框实体类
    /// </summary>
    class SearchTextBox:BindableBase, IDataErrorInfo
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SearchTextBox()
        {
            SearchQuestionNumber = "0";
        }

        /// <summary>
        /// 查询的题号
        /// </summary>
        private string searchQuestionNumber;
        public string SearchQuestionNumber
        {
            get { return searchQuestionNumber; }
            set { SetProperty(ref searchQuestionNumber, value.Replace(" ","")); }
        }

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

        public bool IsValid()
        {
            return string.IsNullOrEmpty(IsValid("SearchQuestionNumber"));
        }

        private string IsValid(string propertyName)
        {
            switch (propertyName)
            {
                case "SearchQuestionNumber":
                    if (string.IsNullOrWhiteSpace(SearchQuestionNumber) || string.IsNullOrEmpty(SearchQuestionNumber))
                    {
                        return "内容不能为空";
                    }
                    if (!isValidNumber(SearchQuestionNumber))
                    {
                        return "只能输入正整数且位数不超过10位";
                    }
                    break;
                default:
                    break;
            }
            return null;
        }
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
            if (_value < 0)
            {
                return false;
            }
            return true;
        } 
        #endregion
    }
}

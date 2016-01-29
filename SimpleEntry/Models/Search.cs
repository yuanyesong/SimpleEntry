
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.ComponentModel;

namespace SimpleEntry.Models
{
    /// <summary>
    /// 问卷答案的查询试题Model类
    /// </summary>
    class Search:BindableBase,IDataErrorInfo
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        private Search()
        {
            searchRecord = "0";
        }

        private static readonly Search instance = new Search();
        public static Search Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// 查询的记录号
        /// </summary>
        private string searchRecord;
        public string SearchRecord
        {
            get { return searchRecord; }
            set { SetProperty(ref searchRecord, value.Replace(" ", "")); }
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
            return string.IsNullOrEmpty(IsValid("SearchRecord"));
        }

        private string IsValid(string propertyName)
        {
            switch (propertyName)
            {
                case "SearchRecord":
                    if (string.IsNullOrWhiteSpace(SearchRecord) || string.IsNullOrEmpty(SearchRecord))
                    {
                        return "内容不能为空";
                    }
                    if (!isValidNumber(SearchRecord))
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

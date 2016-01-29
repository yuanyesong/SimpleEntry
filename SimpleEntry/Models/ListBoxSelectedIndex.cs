using Microsoft.Practices.Prism.Mvvm;
using System.Collections.Generic;

namespace SimpleEntry.Models
{
    /// <summary>
    /// 用于录入的跳转
    /// 用于存储题号与DataEntryFormViewModel的ListBox的SelectedIndex的字典
    /// 单件模式
    /// </summary>
    class ListBoxSelectedIndex : BindableBase
    {
        private static readonly ListBoxSelectedIndex instance = new ListBoxSelectedIndex();

        /// <summary>
        /// DataEntryFormViewModel的ListBox的SelectedIndex
        /// </summary>
        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { SetProperty(ref selectedIndex, value); }
        }

        /// <summary>
        /// 题号与SelectedIndex对应的字典
        /// </summary>
        private Dictionary<string, int>  dicQIndex;
        public  Dictionary<string, int>  DicQIndex
        {
            get { return dicQIndex; }
            set { SetProperty(ref dicQIndex, value); }
        }

         /// <summary>
         /// 字典的value
         /// </summary>
        private int keyValue;
        public int KeyValue
        {
            get { return keyValue; }
            set { SetProperty(ref keyValue, value); }
        }
        //private bool isBlank;

        //public bool IsBlank
        //{
        //    get { return isBlank; }
        //    set { SetProperty(ref isBlank, value); }
        //}


        private ListBoxSelectedIndex()
        {
            this.SelectedIndex = 0;
            this.DicQIndex=new Dictionary<string,int>() ;
            this.KeyValue = 0;
            //this.IsBlank = true;
        }
        public static ListBoxSelectedIndex Instance
        {
            get
            {
                return instance;
            }
        }
    }
}

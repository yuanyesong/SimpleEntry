using Microsoft.Practices.Prism.Mvvm;
using SimpleEntry.ViewModels.Windows;

namespace SimpleEntry.Models
{
    /// <summary>
    /// 用于状态栏更新
    /// 单件模式
    /// </summary>
    class Status:BindableBase
    {
        private static readonly Status instance = new Status();
        private string statusMessage;
        public string StatusMessage
        {
            get { return statusMessage; }
            set { SetProperty(ref statusMessage, value); }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private Status()
        {
            StatusMessage = "就绪";
        }
        public  static Status Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// 显示状态信息
        /// </summary>
        /// <param name="StatusMessage"></param>
        public void ShowStatus(string StatusMessage)
        {
            Status.Instance.StatusMessage = StatusMessage;
            DispatcherHelper.DoEvents();
        }
    }
}

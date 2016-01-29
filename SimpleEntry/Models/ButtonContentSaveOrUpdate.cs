using Microsoft.Practices.Prism.Mvvm;

namespace SimpleEntry.Models
{
    /// <summary>
    /// 用于绑定到保存/更新按钮文本的Model类
    /// 如果存在则显示“更新”，不存在则显示“保存”
    /// </summary>
    class ButtonContentSaveOrUpdate:BindableBase
    {
        /// <summary>
        /// 显示按钮是保存还是更新
        /// </summary>
        private string saveOrUpdateName;
        public string SaveOrUpdateName
        {
            get { return saveOrUpdateName; }
            set { SetProperty(ref saveOrUpdateName, value); }
        }

    }
}

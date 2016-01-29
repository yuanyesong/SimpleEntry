using Microsoft.Practices.Prism.Mvvm;

namespace SimpleEntry.Models
{
    /// <summary>
    /// 单件模式类，判断QuestionField是否已存在用于验证
    /// </summary>
    class IsQuestionFieldExist:BindableBase
    {
        private static IsQuestionFieldExist instance = new IsQuestionFieldExist();
        private bool isExist;
        public bool IsExist
        {
            get { return isExist; }
            set { SetProperty(ref isExist, value); }
        }

        private IsQuestionFieldExist()
        {
            IsExist = false;
        }
        public static IsQuestionFieldExist Instance
        {
            get
            {
                return instance;
            }
        }
    }
}

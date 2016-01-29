using Microsoft.Practices.Prism.Mvvm;

namespace SimpleEntry.Models
{
    /// <summary>
    /// QuestionAnwser实体Model类
    /// </summary>
    class QuestionAnwser:BindableBase
    {
        //private int questionNumber;//题号

        //public int QuestionNumber
        //{
        //    get { return questionNumber; }
        //    set { SetProperty(ref questionNumber, value); }
        //}
        //private int singleAnwser;//单选题答案

        //public int SingleAnwser
        //{
        //    get { return singleAnwser; }
        //    set { SetProperty(ref singleAnwser, value); }
        //}
        //private int[] multiAnwser;//多选题答案

        //public int[] MultiAnwser
        //{
        //    get { return multiAnwser; }
        //    set { SetProperty(ref multiAnwser, value); }
        //}
        //private int trueOrFalseAnwser;//判断题答案

        //public int TrueOrFalseAnwser
        //{
        //    get { return trueOrFalseAnwser; }
        //    set { SetProperty(ref trueOrFalseAnwser, value); }
        //}
        //private string otherOptionAnwser;//其他选项和填空题答案

        //public string OtherOptionAnwser
        //{
        //    get { return otherOptionAnwser; }
        //    set { SetProperty(ref otherOptionAnwser, value); }
        //}

        /// <summary>
        /// 题号
        /// </summary>
        public int QuestionNumber { get; set; }

        /// <summary>
        /// 单选题答案
        /// </summary>
        public int SingleAnwser { get; set; }

        /// <summary>
        /// 多选题答案
        /// </summary>
        public int[] MultiAnwser { get; set; }

        /// <summary>
        /// 判断题答案
        /// </summary>
        public int TrueOrFalseAnwser { get; set; }

        /// <summary>
        /// 其他选项答案及填空题答案
        /// </summary>
        public string OtherOptionAnwser { get; set; }

        /// <summary>
        /// 答案记录号
        /// </summary>
        public int Record { get; set; }
    }
}

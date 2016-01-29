
namespace SimpleEntry.Models
{
    /// <summary>
    /// QuestionType实体Model类
    /// </summary>
   public class QuestionType
    {
       /// <summary>
       /// 题型编号
       /// 单选题0，多选题1，判断题2，填空题3
       /// </summary>
        public int QuestionTypeID { get; set; }

       /// <summary>
       /// 题型
        /// 单选题、多选题、判断题、填空题
       /// </summary>
        public string QType { get; set; }
    }
}

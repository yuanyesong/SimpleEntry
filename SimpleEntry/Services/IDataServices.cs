using SimpleEntry.Models;

namespace SimpleEntry.Services
{
    /// <summary>
    /// 获取问卷信息服务
    /// </summary>
    interface IDataServices
    {
        QuestionInfo GetAllQuestionInfo(int questionNumber, string DataSource);
    }
}

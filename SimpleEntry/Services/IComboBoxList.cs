using SimpleEntry.Models;
using System.Collections.Generic;

namespace SimpleEntry.Services
{
    /// <summary>
    /// 初始化题型和数据类型
    /// </summary>
    public interface IComboBoxList
    {
        List<QuestionType> QuestionTypes();
        List<DataType> DataTypes();
    }
}

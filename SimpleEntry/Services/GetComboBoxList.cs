using SimpleEntry.Models;
using System.Collections.Generic;

namespace SimpleEntry.Services
{
    /// <summary>
    /// 初始化题型与数据类型
    /// </summary>
    class GetComboBoxList : IComboBoxList
    {
        public List<QuestionType> QuestionTypes()
        {
            return new List<QuestionType> 
            {
                new QuestionType {QuestionTypeID = 0, QType="单选题"},
                new QuestionType {QuestionTypeID = 1, QType="多选题"},
                new QuestionType {QuestionTypeID = 2, QType="判断题"},
                new QuestionType {QuestionTypeID = 3, QType="填空题"}
            };
        }
        public List<DataType> DataTypes()
        {
            return new List<DataType>
            {
                new DataType {DataTypeID=0,DType="数字"},
                new DataType {DataTypeID=1,DType="文本"},
                new DataType {DataTypeID=2,DType="日期"}
            };
        }
    }
}

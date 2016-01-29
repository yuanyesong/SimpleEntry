
namespace SimpleEntry.Models
{
    /// <summary>
    /// 数据类型Model类
    /// </summary>
    public class DataType
    {
        /// <summary>
        /// 数据类型编号
        /// 数字0，文本1，日期2
        /// </summary>
        public int DataTypeID { get; set; }

        /// <summary>
        /// 数据类型
        /// 数字、文本、日期
        /// </summary>
        public string DType { get; set; }
    }
}

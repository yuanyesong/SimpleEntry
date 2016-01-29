
namespace SimpleEntry.Models
{
    /// <summary>
    /// 控制ChromeTab的接口
    /// </summary>
    interface ITab
    {
        /// <summary>
        /// 选项卡序号
        /// </summary>
        int TabNumber { get; set; }

        /// <summary>
        /// 选项卡名字
        /// </summary>
        string TabName { get; set; }
    }
}

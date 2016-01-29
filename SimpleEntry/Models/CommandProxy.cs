using Microsoft.Practices.Prism.Commands;

namespace SimpleEntry.Models
{
    /// <summary>
    /// 命令代理静态类：一次执行多个命令
    /// </summary>
    public static class CommandProxy
    {
        /// <summary>
        /// 保存一份问卷的答案
        /// </summary>
        public static CompositeCommand SaveAllEntryDataCommand = new CompositeCommand();

        /// <summary>
        /// 跳转到第一份问卷的答案
        /// </summary>
        public static CompositeCommand MoveToFirstRecordCommand = new CompositeCommand();

        /// <summary>
        /// 跳转到最后一份问卷的答案
        /// </summary>
        public static CompositeCommand MoveToLastRecordCommand = new CompositeCommand();

        /// <summary>
        /// 前往前一份问卷的答案
        /// </summary>
        public static CompositeCommand MoveToPreviousRecordCommand = new CompositeCommand();

        /// <summary>
        /// 前往下一份问卷的答案
        /// </summary>
        public static CompositeCommand MoveToNextRecordCommand = new CompositeCommand();

        /// <summary>
        /// 删除一份问卷的答案
        /// </summary>
        public static CompositeCommand DeleteAllRecordCommand = new CompositeCommand();

        /// <summary>
        /// 查询一份问卷的答案
        /// </summary>
        public static CompositeCommand SearchAllRecordCommand = new CompositeCommand();
    }
}

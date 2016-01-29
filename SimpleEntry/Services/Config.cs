
namespace SimpleEntry.Services
{
    /// <summary>
    /// 数据库路径实体Model类
    /// </summary>
    class Config
    {
        public string DataBaseFile;
        public string DataSource
        {
            get
            {
                return string.Format("data source={0}", DataBaseFile);
            }
        }
    }
}

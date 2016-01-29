using SimpleEntry.Models;

namespace SimpleEntry.ViewModels
{
    class AboutViewModel: ITab
    {
        public int TabNumber { get; set; }
        public string TabName { get; set; }
        public AboutViewModel()
        {
            this.TabName = "关于";
        }
    }
}

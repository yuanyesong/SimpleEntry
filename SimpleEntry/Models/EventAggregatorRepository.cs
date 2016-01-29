using Microsoft.Practices.Prism.PubSubEvents;

namespace SimpleEntry.Models
{
    /// <summary>
    /// 用于在DataEntryFormViewModel与DataEntryViewModel之间传递查询记录号
    /// 单件模式，EventAggregatorRepository
    /// </summary>
    class EventAggregatorRepository
    {
        private static readonly EventAggregatorRepository instance = new EventAggregatorRepository();
        public IEventAggregator eventAggregator;

        public EventAggregatorRepository()
        {
            eventAggregator = new EventAggregator();
        }

        public static EventAggregatorRepository Instance
        {
            get
            {
                return instance;
            }
        }
    }
}

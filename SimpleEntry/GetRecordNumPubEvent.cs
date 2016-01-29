using Microsoft.Practices.Prism.PubSubEvents;

namespace SimpleEntry
{
    /// <summary>
    /// EventAggregator广播和订阅
    /// </summary>
    class GetRecordNumPubEvent:PubSubEvent<int>
    {
    }
}

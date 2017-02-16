using System.Collections.Concurrent;
using System.Collections.Generic;
namespace EventHub.Auto.Doc {
    public class EhConnectionInfo {
            public EhConnectionInfo()
            {
                EventHubs = new ConcurrentDictionary<string,EventHub>();
            }
            public string Namespace { get; set; }
            public string Key { get; set; }
            public string ConsumerGroup { get; set; }
            public string Id { get; set; }
            public string ConnectionString { get; set; }
            public ConcurrentDictionary<string,EventHub> EventHubs {get;set;}
    }

    public class EventHub {
        public EventHub() {
            EventTypes = new ConcurrentDictionary<string,EventType>();
        }
        public string Name { get; set; }
        public ConcurrentDictionary<string,EventType> EventTypes { get; set; }
    }

    public class EventType {
        public EventType() {
            Properties = new HashSet<string>();
            Examples = new ConcurrentBag<string>();
        }
        public string Name { get; set; }
        public HashSet<string> Properties { get; set;}
        public ConcurrentBag<string> Examples { get; set; }
    }

}

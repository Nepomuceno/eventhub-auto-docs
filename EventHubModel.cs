using System.Collections.Generic;
namespace EventHub.Auto.Doc {
    public class EhConnectionInfo {
            public EhConnectionInfo()
            {
                EventHubs = new List<string>();
            }
            public string Namespace { get; set; }
            public string Key { get; set; }
            public string ConsumerGroup { get; set; }
            public string Id { get; set; }
            public string ConnectionString { get; set; }
            public List<string> EventHubs {get;set;}
    }
    public class EhNamespace {
        public EhNamespace() {
            EventHubs = new List<EventHub>();
        }
        public string Name { get; set; }
        public List<EventHub> EventHubs { get; set; }
    }

    public class EventHub {
        public EventHub() {
            EventTypes = new List<EventType>();
        }
        public string Name { get; set; }
        public List<EventType> EventTypes { get; set; }
    }

    public class EventType {
        public EventType() {
            Properties = new HashSet<string>();
            Examples = new List<string>();
        }
        public string Name { get; set; }
        public HashSet<string> Properties { get; set;}
        public List<string> Examples { get; set; }
    }

}

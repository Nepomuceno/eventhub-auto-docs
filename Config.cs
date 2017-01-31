using System.Collections.Generic;

namespace EventHub.Auto.Doc {
    public class Config {
        public Config()
        {
            Subscriptions = new List<SubscriptionInfo>();
        }
        public string StorageAccount { get; set; }
        public string StorageKey { get; set; }
        public bool SendSlackInfo { get; set; }
        public string SlackUrl { get; set; }
        public List<SubscriptionInfo> Subscriptions { get; set; }
    }
    public class SubscriptionInfo {
        public string SubscriptionId { get; set; }
        public string Name { get; set; }
    }
}
/*
{
    "subscriptions" : [
                {
                    "subscriptionId":"904ad912-63ba-41fd-bb70-c7d23b5c8674",
                    "name": "DEVELOPMENT IT AZURE (EA)"
                },
                {
                    "subscriptionId":"af515c4f-82cd-4dc7-a360-8c855842ff60",
                    "name": "DEVELOPMENT BIGDATA (EA MSDN)"
                },
                {
                    "subscriptionId":"4b26ca69-1cf0-44c9-91cc-1ba482f8b06e",
                    "name": "DEVELOPMENT TOOLS (EA MSDN)"
                },
                {
                    "subscriptionId":"60881ba6-11ee-4eeb-a90c-6e6e218ba086",
                    "name": "Shopomo PRE (MSDN EA)"
                },
                {
                    "subscriptionId":"97d3edd9-93d4-4b12-bbe3-6c41922a9b8c",
                    "name": "DEVELOPMENT SHOPOMO (EA MSDN)"
                },
                {
                    "subscriptionId":"b1921b69-4667-4cc6-a9e7-5518f4a80ca1",
                    "name": "Ve Preproduction (MSDN EA)"
                },
                {
                    "subscriptionId":"c6b7d7fc-ddbd-4619-bbd5-75122dc8634e",
                    "name": "Travioor PRE (MSDN EA)"
                },
                {
                    "subscriptionId":"d3579878-2c08-419b-bf49-42251129a424",
                    "name": "Ve Big Data Platform PRE (MSDN EA)"
                },
                {
                    "subscriptionId":"0c62076e-ddf0-4dbe-8470-bc40784f0fea",
                    "name": "Travioor PRO (EA)"
                },
                {
                    "subscriptionId":"206878b8-5f28-476e-acc8-1cff1b03efe3",
                    "name":"Ve Production ASIA (EA)"
                },
                {
                    "subscriptionId":"22f1bdf4-3e6d-4fa4-8c0a-fa5940198e33",
                    "name":"Ve Production EUROPE v2 (EA)"
                },
                {
                    "subscriptionId":"5158e0fd-394a-4434-9908-8bf7ecad69ea",
                    "name":"Ve Creative (EA)"
                },
                {
                    "subscriptionId":"999bf067-9b94-48e9-be87-507a01a152ab",
                    "name":"Ve Big Data Platform USA"
                },
                {
                    "subscriptionId":"c44f9eb0-3180-4942-8315-612831fd7d3f",
                    "name":"Ve Production EUROPE (EA)"
                },
                {
                    "subscriptionId":"f4d34235-c902-4739-b830-84a575ff6e50",
                    "name":"Ve Production North America (EA)"
                },
                {
                    "subscriptionId":"f5a9b9dd-88a8-4e1d-87ab-2ca3a19a08a0",
                    "name":"Shopomo PRO (EA)"
                }
    ],
    "storageAccount":"4b2161westeurope",
    "storageKey":"9gIzTWi1LuWiqHuWISnlLopsTq7yQNi+265f3PZwVTBJMRC92TidmdOoSId4xwzVH76nKeR14abLpTgbzH92Dg==",
    "sendSlackInfo": true,
    "slackUrl": "https://hooks.slack.com/services/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"
}
*/
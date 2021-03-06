﻿using System;
using System.Collections.Generic;
using Microsoft.Azure.Management.EventHub;
using Microsoft.Azure.Management.EventHub.Models;
using Microsoft.Azure.Management.Resources;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace EventHub.Auto.Doc
{

    public class Program
    {
        private const string ConsumerGroupName = "auto-doc";
        private static Dictionary<string,EhConnectionInfo> AllEventHubs = new Dictionary<string,EhConnectionInfo>();
        
        public static void Main(string[] args)
        {
            var configText = File.ReadAllText("./base.config.azure");
            Config config = JsonConvert.DeserializeObject<Config>(configText);
            var tokenCredentials = GetAzureConnection("./cred.azure");
            
            var connectionString = $"DefaultEndpointsProtocol=https;AccountName={config.StorageAccount};AccountKey={config.StorageKey}";
            CloudStorageAccount storageAccountClient = CloudStorageAccount.Parse(connectionString);
            
            foreach (var subscription in config.Subscriptions)
            {
                var ehClient = new EventHubManagementClient(tokenCredentials);
                ehClient.SubscriptionId = subscription.SubscriptionId;
                var resources = ehClient.Namespaces.ListBySubscription();
                foreach(var resource in resources){
                    EhConnectionInfo conn = GetConnectionInfo(ehClient,resource);
                    AllEventHubs.Add(resource.Id,conn);
                }
                foreach(var ehNamespace in AllEventHubs.Values)
                {
                    Parallel.ForEach(ehNamespace.EventHubs, eh => 
                    {
                        var nsName = ehNamespace.Namespace.Substring(0,Math.Min(ehNamespace.Namespace.Length,29));
                        var ehName = eh.Key.Substring(0,Math.Min(eh.Key.Length,29));
                        var containerName = $"doc-{nsName}-{eh}";
                        CloudBlobClient blobClient = storageAccountClient.CreateCloudBlobClient();
                        CloudBlobContainer container = blobClient.GetContainerReference(containerName);
                        container.CreateIfNotExistsAsync().Wait();
                        ConsumeData(eh,ehNamespace.ConnectionString,connectionString,containerName, $"{ehNamespace.Namespace}-{eh}");
                    });
                }
            }
            Console.WriteLine("End App!");
            Console.ReadLine();
        }
        private static Dictionary<string,string> ReadCredFile(string credPath)
        {
            var result = new Dictionary<string,string>();
            var lines = File.ReadAllLines(credPath);
            foreach (var line in lines)
            {
                result.Add(line.Split('=')[0],line.Split('=')[1]);
            }
            return result;
        }
        public static TokenCredentials GetAzureConnection(string credPath){
            var data = ReadCredFile(credPath);
            var authToken = GetAuthorizationToken(data["client"],data["key"],data["tenant"]);  
            return new TokenCredentials(authToken);
        }

        public static void ConsumeData(
        EventHub eh, 
        string ehConnectionString, 
        string storageConnectionString,
        string storageContainerName, string ehNamespace)
        {
            Console.WriteLine("Registering EventProcessor...");

            var eventProcessorHost = new EventProcessorHost(
                eh.Name,
                ConsumerGroupName,
                ehConnectionString,
                storageConnectionString,
                storageContainerName);

            // Registers the Event Processor Host and starts receiving messages
            eventProcessorHost
            .RegisterEventProcessorFactoryAsync
            (new EventDocProcessorFactory(ehNamespace))
            .Wait();
            Console.WriteLine("Starting Reading Messages");
            Task.Delay(5000).Wait();

            // Disposes of the Event Processor Host
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }

        private static EhConnectionInfo GetConnectionInfo(EventHubManagementClient ehClient, NamespaceResource resource)
        {
            var result = new EhConnectionInfo();
            result.Id = resource.Id;
            result.Namespace = resource.Name;
            var splitId = resource.Id.Split('/');
            var resourceGroup = splitId[4];
            var ehNamespace = splitId[8]; 
            var Keys = ehClient.Namespaces.ListKeys(resourceGroup,ehNamespace,"RootManageSharedAccessKey");
            result.ConnectionString = Keys.PrimaryConnectionString;
            result.Key = Keys.PrimaryKey;
            var eventHubs = ehClient.EventHubs.ListAll(resourceGroup,ehNamespace);
            Parallel.ForEach(eventHubs, ev => 
            {
                var param = new ConsumerGroupCreateOrUpdateParameters(ev.Location);
                param.Name = ConsumerGroupName;
                try
                {
                    ehClient.ConsumerGroups.CreateOrUpdate(resourceGroup,ehNamespace,ev.Name,ConsumerGroupName,param);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error connecting to {ev.Name} Message: {ex.Message}");
                }
                var eventHub = new EventHub(){
                    Name = ev.Name
                };
                result.EventHubs.AddOrUpdate(ev.Name,eventHub,(key,old) => {
                    if(old != null)
                    {
                        old.Name = eventHub.Name;
                        return old;
                    }
                    return eventHub;
                });
            });
            return result;
            
        }

        private static string GetAuthorizationToken(string clientId, string serverPrincipalPassword,string azureTenantId)
        {
            ClientCredential cc = new ClientCredential(clientId, serverPrincipalPassword);
            var context = new AuthenticationContext("https://login.windows.net/" + azureTenantId);
            var result = context.AcquireTokenAsync("https://management.azure.com/", cc);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }
            return result.Result.AccessToken;
        }
        public class EventDocProcessorFactory : IEventProcessorFactory
        {
            private readonly string _ehName;
            public EventDocProcessorFactory(string ehName) {
                _ehName = ehName;
            }
            public IEventProcessor CreateEventProcessor(PartitionContext context)
            {
                return new SimpleEventProcessor(_ehName);
            }
        }

        public class SimpleEventProcessor : IEventProcessor
        {
            private string _ehName;
            public SimpleEventProcessor(string ehName) {
                _ehName = ehName;
            }
            public Task CloseAsync(PartitionContext context, CloseReason reason)
            {
                return Task.FromResult<object>(null);
            }

            public Task OpenAsync(PartitionContext context)
            {
                return Task.FromResult<object>(null);
            }

            public Task ProcessErrorAsync(PartitionContext context, Exception error)
            {
                Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
                return Task.FromResult<object>(null);
            }

            public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
            {
                dynamic result;
                foreach (var eventData in messages)
                {
                    var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    foreach(var key in eventData.Properties)
                    {
                        if(key.Key.StartsWith("x-opt-"))
                            continue;
                        if(key.Key.Equals("type",StringComparison.OrdinalIgnoreCase))
                        {
                            result.Type = 
                        }
                    }
                    try {
                        var model = JsonConvert.DeserializeObject<dynamic>(data);
                        Console.WriteLine($"[EventProperties]:");
                        foreach(var key in model)
                        {
                            Console.WriteLine(key.Name);
                        }
                    } catch {
                        Console.WriteLine($"not a valid json: {data}");
                    }
                }
                await context.CheckpointAsync();
            }
        }
    }
}

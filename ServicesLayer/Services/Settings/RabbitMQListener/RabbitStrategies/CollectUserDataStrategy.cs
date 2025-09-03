using ContractsLayer.Base;
using ContractsLayer.Common;
using ContractsLayer.Models.Clustering;
using Microsoft.Extensions.Configuration;
using ServicesLayer.IServices;
using ServicesLayer.Services.Settings.RabbitMQListener.RabbitStrategies.Base;
using static ContractsLayer.Common.DefaultEnums;

namespace ServicesLayer.Services.Settings.RabbitMQListener.RabbitStrategies
{
    /// <summary>
    /// стратегия сбора данных пользователей для выполнения кластеризации
    /// кластеризация выполняется отдельным сервисом
    /// </summary>
    public class CollectUserDataStrategy : IRabbitStrategy
    {
        private readonly IServiceManager _serviceManager;
        private readonly IConfiguration _configuration;
        public CollectUserDataStrategy(IServiceManager serviceManager, IConfiguration config) 
        {
            _serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
            _configuration = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<BaseModel> ExecuteStrategy()
        {
            BaseModel result = null;
            var usersList = await _serviceManager._integrationService.GetFullUserDataForClustering();
            if (usersList != null && usersList.Count > 0)
            {
                var serializableTypes = new[] { typeof(FullUserDataForClustering), typeof(TransportMessageWithBody<List<FullUserDataForClustering>>) };
                var rabbitQueueName = _configuration["Rabbit:clusteringQueue"];
                double chunkSize = Math.Floor(Convert.ToDouble(_configuration["Rabbit:chunkSizeMb"]) * 1048576);
                long totalSize = CommonUtilities.GetSizeOfManagedObject(usersList, null);
                if (totalSize > chunkSize)
                {
                    double coefficient = (double)totalSize / chunkSize;
                    int approximateCountOfUsersChunked = (int)Math.Floor(usersList.Count / coefficient);
                    int count = 0;
                    List<TransportMessage> messageList = new List<TransportMessage>(); 
                    while(count < usersList.Count)
                    {
                        var partition = usersList.Skip(count).Take(approximateCountOfUsersChunked).ToList();
                        var tm = new TransportMessageWithBody<List<FullUserDataForClustering>>
                        {
                            TransportMessageType = (int)TransportMessageType.node_communication,
                            TaskDescription = null,
                            MessageHeader = "Список пользователей для проведения кластеризации (один чанк)",
                            MessageBody = partition,
                            IsLastChunk = false
                        };
                        messageList.Add(tm);
                        count += approximateCountOfUsersChunked; 
                    }
                    messageList.Add(new TransportMessageWithBody<List<FullUserDataForClustering>>
                    {
                        TransportMessageType = (int)TransportMessageType.node_communication,
                        TaskDescription = null,
                        MessageHeader = "Конец обмена сообщениями",
                        MessageBody = null
                    });
                    result = await _serviceManager._integrationService.ExecutePublishRabbitMQ(messageList.ToArray(), rabbitQueueName,
                        serializableTypes, IsChunked: true);
                }
                else
                {
                    var tm = new TransportMessageWithBody<List<FullUserDataForClustering>>
                    {
                        TransportMessageType = (int)TransportMessageType.node_communication,
                        TaskDescription = null,
                        MessageHeader = "Список пользователей для проведения кластеризации",
                        MessageBody = usersList
                    };
                    result = await _serviceManager._integrationService.ExecutePublishRabbitMQ(new TransportMessage[]{tm},
                        rabbitQueueName, serializableTypes                       
                    );
                }
                return result;               
            }
            else return null;
        }
    }
}

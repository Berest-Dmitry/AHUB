using ContractsLayer.Base;
using ContractsLayer.Dtos;
using ContractsLayer.Models.Clustering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RepositoryLayer.IRepositories;
using ServicesLayer.IServices;
using ServicesLayer.Services.Settings.RabbitMQListener;
using System.Xml.Serialization;

namespace ServicesLayer.Services
{
    /// <summary>
    /// сервис интеграции со сторонними системами
    /// </summary>
    public class IntegrationService: IIntegrationService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;

        public IntegrationService(IRepositoryManager repositoryManager, IConfiguration config)
        {
            _repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
            _configuration = config ?? throw new ArgumentNullException(nameof(config));

            var logFactory = LoggerFactory.Create(loggingBuilder =>
            loggingBuilder.SetMinimumLevel(LogLevel.Debug)
                .AddConsole()
            );
            _logger = logFactory.CreateLogger<PostsService>();
        }

        /// <summary>
        /// метод первоначальной установки соединения с реббитом
        /// </summary>
        /// <param name="QueueName">название очереди</param>
        /// <returns></returns>
        private bool EstablishConnectionRabbit(string QueueName)
        {
            try
            {
                var factory = new ConnectionFactory { HostName = _configuration["Rabbit:host"] };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: QueueName,
                        durable: true,
                        autoDelete: false,
                        exclusive: false,
                    arguments: null);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred when trying to establish connection to RabbitMQ.\n" +
                   $"See details: {ex.Message}\n {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// базовый вариант метода публикации сообщения в Реббит
        /// </summary>
        /// <param name="messageToSend">сообщение для отправки в очередь</param>
        /// <returns></returns>
        private async Task<BaseModel> BasicPublishRabbitMQ(TransportMessage messageToSend, string QueueName, Type[] serializableTypes)
        {
            try
            {
                byte[] buffer = null;
                using (var ms = new MemoryStream())
                {
                    var serializer = new XmlSerializer(typeof(TransportMessage), serializableTypes);
                    serializer.Serialize(ms, messageToSend);
                    buffer = ms.ToArray();
                    ms.Close();                  
                }
                _channel.BasicPublish(mandatory: true, exchange: "", routingKey: QueueName, basicProperties: null, body: buffer);
                _logger.Log(LogLevel.Information, $"Successfully sent message at {DateTime.Now.ToString("HH.mm.ss")}");
                return new BaseModel();
            }
            catch(Exception ex)
            {
                _logger.LogError($"An error occurred when trying to form and publish message of type {messageToSend.TransportMessageType}.\n" +
                    $"See details: {ex.Message}\n {ex.StackTrace}");
                return new BaseModel(ex);
            }
        }

        public async Task<BaseModel> ExecutePublishRabbitMQ(TransportMessage[] messagesToSend, string QueueName, Type[] serializableTypes, bool IsChunked = false)
        {
            bool isConnected = false;
            try
            {
                isConnected = EstablishConnectionRabbit(QueueName);
                if (!isConnected)
                {
                    _logger.LogError("Failed to connect to RabbitMQ");
                    return new BaseModel(new Exception("Failed to connect to RabbitMQ"));
                }

                if (IsChunked)
                {
                    BaseModel result = null;
                    for (int i = 0; i < messagesToSend.Length; i++)
                    {
                        result = await BasicPublishRabbitMQ(messagesToSend[i], QueueName, serializableTypes);
                        if (result.Result != ContractsLayer.Common.DefaultEnums.Result.ok)
                        {
                            throw result.Error;
                        }
                    }
                }
                else
                {
                    await BasicPublishRabbitMQ(messagesToSend.FirstOrDefault(), QueueName, serializableTypes);
                }
                return new BaseModel();
            }
            catch (Exception ex)
            {            
                return new BaseModel(ex);
            }
            finally
            {
                if(isConnected)
                {
                    _channel.Close();
                    _connection.Close();
                }
                
            }
        }

        public async Task<List<FullUserDataForClustering>> GetFullUserDataForClustering()
        {
            try
            {
                var userData = await _repositoryManager._usersRepository
                    .GetUsersWithLinkedData();

                var resultingList = new List<FullUserDataForClustering>();
                foreach ( var user in userData )
                {
                    resultingList.Add(new FullUserDataForClustering
                    {
                        firstName = user.firstName,
                        lastName = user.lastName,
                        gender = user.gender,
                        educationInfo = user.educationInfo,
                        registrationDate = user.dateTimeAdded,
                        birthday = user.birthday.GetValueOrDefault(),
                        outerServiceId = user.id,
                        userComments = ObjectMapper.Mapper.Map<List<CommentClusteringModel>>(user.Comments),
                        userPosts = ObjectMapper.Mapper.Map<List<PostClusteringModel>>(user.Posts),
                    });
                }
                _logger.Log(LogLevel.Information, $"Successfully retrievied user data, total users count: {resultingList.Count}");
                return resultingList;

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while attempting to read user data from DB: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        
    }
}

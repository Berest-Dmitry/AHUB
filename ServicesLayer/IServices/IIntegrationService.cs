using ContractsLayer.Base;
using ContractsLayer.Models.Clustering;
using ServicesLayer.Services.Settings.RabbitMQListener;

namespace ServicesLayer.IServices
{
    /// <summary>
    /// интерфейс сервиса интеграции со сторонними системами
    /// </summary>
    public interface IIntegrationService
    {
        /// <summary>
        /// метод получения инф-ии по пользователям для проведения кластеризации
        /// </summary>
        /// <returns></returns>
        Task<List<FullUserDataForClustering>> GetFullUserDataForClustering();
      
        /// <summary>
        /// общий метод выполнения процесса отправки сообщений в реббит
        /// </summary>
        /// <param name="messagesToSend">сообщение для отправки в очередь</param>
        /// <param name="QueueName">название очереди</param>
        /// <param name="serializableTypes">сериализуемые типы данных</param>
        /// <param name="IsChunked">флаг - сообщения дробятся на куски</param>
        /// <returns></returns>
        Task<BaseModel> ExecutePublishRabbitMQ(TransportMessage[] messagesToSend, string QueueName, Type[] serializableTypes, bool IsChunked = false);
    }
}

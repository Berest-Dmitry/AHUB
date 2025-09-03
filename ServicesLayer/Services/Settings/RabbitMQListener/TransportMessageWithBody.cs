
namespace ServicesLayer.Services.Settings.RabbitMQListener
{
    /// <summary>
    /// транспортное сообщение с телом
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class TransportMessageWithBody<T> : TransportMessage
        where T : class
    {
        /// <summary>
		/// тело сообщения (в случае, если обмениваемся сообщениями между нодами кластера)
		/// </summary>
		public T? MessageBody { get; set; }
    }
}

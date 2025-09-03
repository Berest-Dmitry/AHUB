using ContractsLayer.Models.Clustering;
using System.Xml.Serialization;

namespace ServicesLayer.Services.Settings.RabbitMQListener
{
	/// <summary>
	/// модель транспортного сообщения
	/// </summary>
	[Serializable]
	[XmlInclude(typeof(TransportMessageWithBody<List<FullUserDataForClustering>>))]
    public class TransportMessage
	{
		/// <summary>
		/// тип транспортного сообщения
		/// </summary>
		public int TransportMessageType { get; set; }

		/// <summary>
		/// описание исполняемой задачи (в случае, если работаем с крон джобой)
		/// </summary>
		public int? TaskDescription { get; set; }

		/// <summary>
		/// заголовок сообщения (в случае, если обмениваемся сообщениями между нодами кластера)
		/// </summary>
		public string? MessageHeader { get; set; }

		/// <summary>
		/// флаг, показывающий на окончание передачи сообщений (последний отрывок сообщения);
		/// в случае, если передается одиночное сообщение, этот флаг true
		/// </summary>
		public bool IsLastChunk { get; set; } = true;
	}
}

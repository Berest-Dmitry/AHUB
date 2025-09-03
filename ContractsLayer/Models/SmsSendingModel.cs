

namespace ContractsLayer.Models
{
	/// <summary>
	/// модель отправки смс-сообщения
	/// </summary>
	public class SmsSendingModel
	{
		/// <summary>
		/// адрас сервера отправки сообщений
		/// </summary>
		public string BaseSmsServerURL { get; set; }

		/// <summary>
		/// идантификатор api для отправки сообщения
		/// </summary>
		public string SmsRuApiId { get; set; }

		/// <summary>
		/// адресат(ы) сообщения
		/// </summary>
		public string Addressee { get; set; }

		/// <summary>
		/// содержимое сообщения
		/// </summary>
		public string MessageContent { get; set; }

		/// <summary>
		/// флаг - отправить несколько сообщений сразу
		/// </summary>
		public bool IsMultiple { get; set; } = false;

		/// <summary>
		/// находтися ли в режиме тестирования (1 - это да, 0 - это нет)
		/// </summary>
		public int TestMode {  get; set; }
	}
}

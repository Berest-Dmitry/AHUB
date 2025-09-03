
using ContractsLayer.Models;

namespace ServicesLayer.IServices
{
	/// <summary>
	/// интерфейс сервиса отправки сообщений
	/// </summary>
	public interface ISmsService
	{
		Task<bool> SendMessage(SmsSendingModel smsSendingModel);
	}
}

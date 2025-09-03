using ContractsLayer.Models;
using Newtonsoft.Json.Linq;
using ServicesLayer.IServices;


namespace ServicesLayer.Services
{
	/// <summary>
	/// сервис отправки сообщений
	/// </summary>
	public class SmsService: ISmsService
	{
		private readonly HttpClient _httpClient;

		public SmsService()
		{
			_httpClient = new HttpClient();
		}

		/// <summary>
		/// метод отправки сообщения на сервис sms.ru
		/// </summary>
		/// <param name="smsSendingModel"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async Task<bool> SendMessage(SmsSendingModel smsSendingModel)
		{
			try
			{

				var parameters = new Dictionary<string, string>
				{
					{ "to", smsSendingModel.Addressee },
					{ "api_id", smsSendingModel.SmsRuApiId },
					{ "json", "1" },
					{ "test", smsSendingModel.TestMode.ToString() },
					{ "msg", smsSendingModel.MessageContent }
				};
				
				var encodedContent = new FormUrlEncodedContent(parameters);
				var response = await _httpClient.PostAsync(smsSendingModel.BaseSmsServerURL + "/send?", encodedContent);
				var content = await response.Content.ReadAsStringAsync();
				var parsedObject = JObject.Parse(content);
				var status_code = parsedObject.SelectToken("$.status_code")?.Value<string>();
				var sms_status_code = parsedObject
					.SelectToken("$.sms." + smsSendingModel.Addressee + ".status_code")?.Value<string>();

				if(status_code == "100" && sms_status_code == "100")
				{
					return true;
				}
				else return false;
				
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}

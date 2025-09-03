using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractsLayer.Base
{
	/// <summary>
	/// расширенная модель ответа на  запрос с сообщениями
	/// </summary>
	public class AdditionalMessageModel: BaseModel
	{
		/// <summary>
		/// дополнительное информационное сообщение
		/// </summary>
		public string AdditionalInformationalMessage { get; set; }

		/// <summary>
		/// дополнительное сообщение об ошибке
		/// </summary>
		public string AdditionalErrorMessage {  get; set; }
	}
}

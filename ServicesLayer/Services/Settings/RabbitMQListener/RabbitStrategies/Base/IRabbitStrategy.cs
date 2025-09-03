using ContractsLayer.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Services.Settings.RabbitMQListener.RabbitStrategies.Base
{
	/// <summary>
	/// интерфейс стратегии выполнения запроса
	/// </summary>
	public interface IRabbitStrategy
	{
		/// <summary>
		/// метод выполнения стратегии
		/// </summary>
		/// <param name="ErrorMsg"></param>
		/// <returns></returns>
		Task<BaseModel> ExecuteStrategy();
	}
}

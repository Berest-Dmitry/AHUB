using ContractsLayer.Base;
using ServicesLayer.IServices;
using ServicesLayer.Services.Settings.RabbitMQListener.RabbitStrategies.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Services.Settings.RabbitMQListener.RabbitStrategies
{
	/// <summary>
	/// стратегия удаления неиспользуемых файлов из хранилища MinIO
	/// </summary>
	public class RemoveUnusedFilesStrategy: IRabbitStrategy
	{
		private readonly IServiceManager _serviceManager;

		public RemoveUnusedFilesStrategy(IServiceManager serviceManager)
		{
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
		}

		/// <summary>
		/// метод выполнения стратегии
		/// </summary>
		/// <returns></returns>
		public async Task<BaseModel> ExecuteStrategy()
		{
			try
			{
				var removeResult = await _serviceManager._filesService.CheckAndRemoveUnusedFiles();
				if(!string.IsNullOrEmpty(removeResult?.ErrorInfo)) {
					var errorMsg = $"An error occured while performing the task: {removeResult?.ErrorInfo} \n Error occurred in service: {nameof(_serviceManager._filesService)}";
					return new BaseModel(new Exception(errorMsg));
				}
				else
				{
					return new BaseModel();
				}	
			}
			catch(Exception ex)
			{
				return new BaseModel(ex);
			}
		}
	}
}

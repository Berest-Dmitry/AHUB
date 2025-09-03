using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServicesLayer.IServices;
using ServicesLayer.Services.Settings.RabbitMQListener.RabbitStrategies;
using ServicesLayer.Services.Settings.RabbitMQListener.RabbitStrategies.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static ContractsLayer.Common.DefaultEnums;

namespace ServicesLayer.Services.Settings.RabbitMQListener
{
	/// <summary>
	/// класс прослушивателя событий Rabbit
	/// </summary>
	public class RabbitMQListener: BackgroundService
	{
		private readonly ILogger<RabbitMQListener> _logger;

		private RabbitMQ.Client.IConnection _connection;

		private readonly IConfiguration _configuration;

		private readonly IServiceScopeFactory _scopeFactory;

		private IModel _channel; 


		public RabbitMQListener(IConfiguration configuration, ILogger<RabbitMQListener> logger, IServiceScopeFactory serviceScopeFactory)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_scopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
			this.ConfigureListener();
		}

		/// <summary>
		/// настройка прослушивателя
		/// </summary>
		private void ConfigureListener() 
		{
			try
			{
				var conFactory = new ConnectionFactory()
				{
					HostName = _configuration["Rabbit:host"],
				};
				_connection = conFactory.CreateConnection();
				_channel = _connection.CreateModel();
				_channel.QueueDeclare(queue: _configuration["Rabbit:queue"], durable: false, exclusive: false, autoDelete: false, arguments: null);
			}
			catch(Exception ex)
			{
				_logger.LogWarning("An error occurred while configuring RabbitMQ listener: " + ex.Message);
			}
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();

			var consumer = new EventingBasicConsumer(_channel);

			consumer.Received += async (ch, ea) =>
			{
				try
				{
					TransportMessage msg = null;
					using (var memStr = new MemoryStream(ea.Body.ToArray()))
					{
						var serializer = new XmlSerializer(typeof(TransportMessage));
						msg = (TransportMessage)serializer.Deserialize(memStr);
						memStr.Close();
						memStr.Dispose();
					}

					if (msg == null)
					{
						_logger.LogWarning("Transport message was null");
						return;
					}

					await RabbitMQEventHandler.HandleCallAsync(msg, _logger, _scopeFactory);

					_channel.BasicAck(ea.DeliveryTag, false);
				}
				catch(Exception ex)
				{
					_logger.LogWarning(ex.Message);
				}
			};

			_channel.BasicConsume(_configuration["Rabbit:queue"], false, consumer);

			return Task.CompletedTask;
		}

		public override void Dispose()
		{
			_channel.Close();
			_connection.Close();
			base.Dispose();
		}
	}

	/// <summary>
	/// внутренний класс обработчика событий
	/// </summary>
	internal static class RabbitMQEventHandler
	{
		internal static IRabbitStrategy _strategy;

		internal static Dictionary<KeyValuePair<int, int>, IRabbitStrategy> strategyPaths;			

		/// <summary>
		/// метод асинхронной обработки транспортного вызова
		/// </summary>
		/// <param name="message"></param>
		/// <param name="logger"></param>
		/// <param name="_scopeFactory"></param>
		/// <returns></returns>
		internal static async Task HandleCallAsync(TransportMessage message, ILogger<RabbitMQListener> logger, IServiceScopeFactory _scopeFactory)
		{
			try
			{
				AsyncServiceScope asyncServiceScope = _scopeFactory.CreateAsyncScope();
				var serviceManager = asyncServiceScope.ServiceProvider.GetRequiredService<IServiceManager>();
				var configuration = asyncServiceScope.ServiceProvider.GetRequiredService<IConfiguration>();

				var messageType = (TransportMessageType)message.TransportMessageType;
				var taskDescription = (TransportTaskType?)message.TaskDescription;
				if (taskDescription == null && messageType == TransportMessageType.cron_job_call)
				{
					throw new ArgumentException("Incorrect transport message!");
				}

				DefineStrategy(messageType, taskDescription ?? TransportTaskType.not_set, serviceManager, configuration);

				var execResult = await _strategy.ExecuteStrategy();

				if(!string.IsNullOrEmpty(execResult?.ErrorInfo))
				{
					logger.LogInformation("An error occurred inside application buisness logic:\n " + execResult?.ErrorInfo);
				}
			}
			catch (Exception ex)
			{
				logger.LogWarning(ex.Message);
			}
		}

		/// <summary>
		/// метод, определяющий стратегию обработки транспортного сообщения
		/// </summary>
		/// <param name="tmt">тип транспортного сообщения</param>
		/// <param name="ttt">тип задачи, переданной для выполнения по таймеру</param>
		internal static void DefineStrategy(TransportMessageType tmt, TransportTaskType ttt, IServiceManager serviceManager, IConfiguration config)
		{
			strategyPaths = new Dictionary<KeyValuePair<int, int>, IRabbitStrategy>
			{
				{ new KeyValuePair<int, int>(0, 0), new RemoveUnusedFilesStrategy(serviceManager) },
				{ new KeyValuePair<int, int>(1, 1), new CollectUserDataStrategy(serviceManager, config) }
			};

			_strategy = strategyPaths[new KeyValuePair<int, int>((int)tmt, (int)ttt)];
        }

		
	}
}

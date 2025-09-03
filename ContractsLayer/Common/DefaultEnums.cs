
using System.ComponentModel;
using System.Reflection;


namespace ContractsLayer.Common
{
	public class DefaultEnums
	{
		/// <summary>
		/// результат выполнения запроса
		/// </summary>
		public enum Result
		{
			ok = 1,
			error = 0
		}

		/// <summary>
		/// причина жалобы
		/// </summary>
		public enum AppealReason
		{
			[Description("Порнография")]
			adult_content = 1,

			[Description("Сцены насилия")]
			violence,

			[Description("Терроризм")]
			terrorism,

			[Description("Призывы к суициду")]
			incitement_to_suicide,

			[Description("Разжигание межнациональной розни")]
			inciting_ethnic_hatred,

			[Description("Спам")]
			spam
		}

		/// <summary>
		/// ствтус жалобы
		/// </summary>
		public enum AppealStatus
		{
			[Description("еще не рассмотрена")]
			not_considered = 0,

			[Description("на рассмотрении")]
			under_consideration,

			[Description("рассмотрена")]
			considered,

			[Description("отклонена")]
			denied
		}

		/// <summary>
		/// тип транспортного сообщения
		/// </summary>
		public enum TransportMessageType
		{
			[Description("вызов службы по расписанию")]
			cron_job_call,

			[Description("коммуникация между узлами распределенной системы")]
			node_communication
		}

		/// <summary>
		/// тип задачи крон джобы, запускаемой асинхронно
		/// </summary>
		public enum TransportTaskType
		{
			[Description("Задача не установлена")]
			not_set = -1,
			[Description("удаление ненужных файлов из БД и внешнего хранилища MinIO")]
			remove_unused_files,
			[Description("сбор данных по пользователям системы для проведения кластеризации")]
			collect_user_data_for_clustering,
		}

		/// <summary>
		/// метод получения описания значения Enum
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetEnumDescription(Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());

			DescriptionAttribute[] attributes =
				(DescriptionAttribute[])fi.GetCustomAttributes(
				typeof(DescriptionAttribute),
				false);

			if (attributes != null &&
				attributes.Length > 0)
				return attributes[0].Description;
			else
				return value.ToString();
		}

		/// <summary>
		/// список разрешенных типов файлов для загрузки
		/// </summary>
		public readonly static Dictionary<string, string> AllowedFileTypes = new Dictionary<string, string>
		{
			{"pdf", "application/pdf"},
			{"txt", "text/plain"},
			{"xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
			{"xls", "application/vnd.ms-excel"},
			{"doc", "application/msword"},
			{"docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
			{"jpeg", "image/jpeg"},
			{"jpg", "image/jpeg"},
			{"png", "image/png"},
		};
	}
}

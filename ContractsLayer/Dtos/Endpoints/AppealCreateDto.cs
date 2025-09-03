
using static ContractsLayer.Common.DefaultEnums;

namespace ContractsLayer.Dtos.Endpoints
{
	public class AppealCreateDto
	{
		/// <summary>
		/// причина жалобы 
		/// </summary>
		public AppealReason reason { get; set; }

		/// <summary>
		/// комментарий  пользователя
		/// </summary>
		public string? comment { get; set; }

		/// <summary>
		/// код пользователя
		/// </summary>
		public Guid userId { get; set; }

		/// <summary>
		/// код сущности, на которую выполнена жалоба
		/// </summary>
		public Guid appealEntityId { get; set; }
	}
}

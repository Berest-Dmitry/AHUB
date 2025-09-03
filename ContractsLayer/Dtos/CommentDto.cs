using ContractsLayer.Base;
using System.ComponentModel.DataAnnotations;


namespace ContractsLayer.Dtos
{
	/// <summary>
	/// модель представления сущности комментария
	/// </summary>
	public class CommentDto: BaseEntityModel
	{
		/// <summary>
		/// текст комментария
		/// </summary>
		[Required]
		[MaxLength(500)]
		public string content { get; set; }

		/// <summary>
		/// код пользователя, опубликовавшего пост
		/// </summary>
		[Required]
		public Guid userId { get; set; }

		/// <summary>
		/// код родительского поста
		/// </summary>
		[Required]
		public Guid postId { get; set; }

		/// <summary>
		/// код родительского комментария
		/// </summary>
		public Guid? parentId { get; set; }

		/// <summary>
		/// дата-время редактирования комментария
		/// </summary>
		public DateTime? dateTimeEdited { get; set; }

		public bool isRemoved { get; set; } = false;
	}
}

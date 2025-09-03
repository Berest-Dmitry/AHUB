

using System.ComponentModel.DataAnnotations;

namespace ContractsLayer.Dtos.Endpoints
{
	/// <summary>
	/// модель создания комментария к посту
	/// </summary>
	public class CommentCreateDto
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
	}
}

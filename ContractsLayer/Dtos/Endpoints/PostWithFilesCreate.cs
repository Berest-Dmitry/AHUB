
namespace ContractsLayer.Dtos.Endpoints
{
	/// <summary>
	/// модель создания/ обновления поста вместе со списком файлов
	/// </summary>
	public class PostWithFilesCreate
	{
		/// <summary>
		/// объект поста (публикации) для создания
		/// </summary>
		public PostCreateDto? PostDto { get; set; }


		/// <summary>
		/// список файлов, прикрепленных к посту
		/// </summary>
		public List<Guid> PostFileIds { get; set; }
	}
}

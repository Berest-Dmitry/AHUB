
namespace ContractsLayer.Dtos.Endpoints
{
	/// <summary>
	/// модель для обновления поста с изменением списка прикрепленных к нему файлов
	/// </summary>
	public class PostWithFilesUpdate
	{
		/// <summary>
		/// иодель поста
		/// </summary>
		public PostUpdateDto PostDto { get; set; }

		/// <summary>
		/// список прикрепленных к посту файлов
		/// </summary>
		public List<Guid> fileIds {  get; set; }
	}
}

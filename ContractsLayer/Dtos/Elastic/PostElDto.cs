

namespace ContractsLayer.Dtos.Elastic
{
	/// <summary>
	/// модель представления для сохранения данных о посте в БД Elastic
	/// </summary>
	public class PostElDto
	{
		/// <summary>
		/// код записи в Elastic
		/// </summary>
		public Guid id { get; set; }

		/// <summary>
		/// название поста
		/// </summary>
		public string title { get; set; }

		/// <summary>
		/// содержимое поста
		/// </summary>
		public string content { get; set; }

		/// <summary>
		/// имя человека/ название компании, от лица которой опубликован пост
		/// </summary>
		public string? publisherName { get; set; }
	}
}

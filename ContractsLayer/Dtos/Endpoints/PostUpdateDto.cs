using System.ComponentModel.DataAnnotations;


namespace ContractsLayer.Dtos.Endpoints
{
	/// <summary>
	/// модель входных данных для редактирования поста
	/// </summary>
	public class PostUpdateDto
	{

		/// <summary>
		/// название поста
		/// </summary>
		[Required]
		public string title { get; set; }

		/// <summary>
		/// содержимое поста
		/// </summary>
		[Required]
		public string content { get; set; }

		/// <summary>
		/// имя человека/ название компании, от лица которой опубликован пост
		/// </summary>
		public string? publisherName { get; set; }

		/// <summary>
		/// URL ссылки, отмеченной в посте
		/// </summary>
		public string? linkURL { get; set; }

		/// <summary>
		/// текст ссылки, отображаемый в посте
		/// </summary>
		public string? linkName { get; set; }

		/// <summary>
		/// физический адрес, к которму относится метка
		/// </summary>
		public string? geoTag { get; set; }

		/// <summary>
		/// код пользователя-публикатора
		/// </summary>
		public Guid userId { get; set; }
	}
}

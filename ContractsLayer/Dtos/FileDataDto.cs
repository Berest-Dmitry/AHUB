using ContractsLayer.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractsLayer.Dtos
{
	/// <summary>
	/// модель представления работы с файлами
	/// </summary>
	public class FileDataDto: BaseEntityModel
	{
		/// <summary>
		/// название файла
		/// </summary>
		public string fileName { get; set; }

		/// <summary>
		/// путь к файлу 
		/// </summary>
		public string filePath { get; set; }

		/// <summary>
		/// тип содержимого файла
		/// </summary>
		public string mediaType { get; set; }

		/// <summary>
		/// размер файла
		/// </summary>
		public string? fileSize { get; set; }

		/// <summary>
		/// название bucket'а, в котором хранится данный файл в облачном хранилище
		/// </summary>
		public string bucketName { get; set; }

		/// <summary>
		/// код сущности-владельца файла
		/// </summary>
		public Guid? entityOwnerId { get; set; }
	}
}

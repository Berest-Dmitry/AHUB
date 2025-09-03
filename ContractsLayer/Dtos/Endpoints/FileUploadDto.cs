using Microsoft.AspNetCore.Http;

namespace ContractsLayer.Dtos.Endpoints
{
	/// <summary>
	/// модель для отправки файла в файловое хранилище
	/// </summary>
	public class FileUploadDto
	{
		public IFormFile formFile {  get; set; }
	}
}

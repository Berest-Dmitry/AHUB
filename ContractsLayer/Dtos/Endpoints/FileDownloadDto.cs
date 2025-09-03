
namespace ContractsLayer.Dtos.Endpoints
{
	public class FileDownloadDto
	{
		public MemoryStream stream {  get; set; }
		public string contentType { get; set; }
		public string fileName { get; set; }
	}
}

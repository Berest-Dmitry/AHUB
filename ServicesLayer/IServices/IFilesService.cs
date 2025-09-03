using ContractsLayer.Base;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using Microsoft.AspNetCore.Http;


namespace ServicesLayer.IServices
{
	/// <summary>
	/// интерфейс сервиса работы с файлами
	/// </summary>
	public interface IFilesService
	{
		Task<BaseModel> UploadFileToMinIO(FileDataDto fileData, Stream ms);

		Task<BaseResponseModel<FileDataDto>> UploadFile(IFormFile formFile);

		Task<BaseResponseModel<FileDataDto>> SaveFileEntry(FileDataDto fileData);

		Task<FileDownloadDto> DownloadFile(string fileName);

		Task<BaseModel> RemoveFileFromMinio(string fileName);

		Task<BaseModel> RemoveFilesFromMinio(List<string> files);

		Task<BaseResponseModel<FileDataDto>> DeleteFileEntry(Guid fileId);

		Task<BaseResponseModel<List<Guid>>> DeleteFileEntries(List<Guid> fileIds);

		Task<BaseResponseModel<FileDownloadDto>> GetFile(Guid fileId);

		Task<BaseResponseModel<BaseModel>> DeleteFile(Guid fileId);

		Task<BaseModel> CheckAndRemoveUnusedFiles();
	}
}

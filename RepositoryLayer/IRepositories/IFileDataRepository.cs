using DomainLayer.Models;
using RepositoryLayer.IRepositories.Base;

namespace RepositoryLayer.IRepositories
{
	/// <summary>
	/// интерфейс репозитория работы с файлами
	/// </summary>
	public interface IFileDataRepository: IRepository<FileData>
	{
		Task<List<FileData>> GetListOfFiles(List<Guid> fileIds);

		Task<FileData> GetFile(Guid fileId);

		Task<FileData> AddFileEntry(FileData file, List<string> propsToChange);

		Task<FileData> DeleteFileEntry(Guid fileId);

		Task<List<Guid>> DeleteFiles(List<Guid> fileIds);

		Task<bool> CheckIfFilesExist(List<Guid> fileIds);
	}
}

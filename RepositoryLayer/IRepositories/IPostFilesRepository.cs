using DomainLayer.Models;
using RepositoryLayer.IRepositories.Base;

namespace RepositoryLayer.IRepositories
{
	/// <summary>
	/// интерфейс репозитория сущности-связки между файлами и постами
	/// </summary>
	public interface IPostFilesRepository: IRepository<PostFile>
	{
		Task<List<Guid>> GetPostFiles(Guid postId);

		Task<PostFile> AddPostFile(Guid postId, Guid fileId);

		Task<List<PostFile>> AddPostFiles(Guid postId, List<Guid> fileIds);

		Task<PostFile> DeletePostFile(Guid postId, Guid fileId);

		Task<List<PostFile>> DeletePostFiles(Guid postId, List<Guid> postFilesIds, bool withCommit = false);
	}
}

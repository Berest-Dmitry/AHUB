using DomainLayer.Data;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.IRepositories;
using RepositoryLayer.Repositories.Base;


namespace RepositoryLayer.Repositories
{
	/// <summary>
	/// репозиторий сущности-связки между файлами и постами
	/// </summary>
	public class PostFilesRepository: Repository<PostFile>, IPostFilesRepository
	{
		public PostFilesRepository(AHUBContext dbContext) : base(dbContext) { }

		/// <summary>
		/// метод получения списка файлов, прикрепленных к посту
		/// </summary>
		/// <param name="postId"></param>
		/// <returns></returns>
		public async Task<List<Guid>> GetPostFiles(Guid postId)
		{
			return await _dbContext.PostFiles.Where(x => x.postId == postId)
				.Select(x => x.fileId).ToListAsync();
		}

		/// <summary>
		/// метод сохранения связи между файлом и постом
		/// </summary>
		/// <param name="postId"></param>
		/// <param name="fileId"></param>
		/// <returns></returns>
		public async Task<PostFile> AddPostFile(Guid postId, Guid fileId)
		{
			var postFileEntry = new PostFile(postId, fileId);
			await AddAsync(postFileEntry);
			return postFileEntry;
		}

		/// <summary>
		/// метод сохранения несколькоих связей между файлами и постом
		/// </summary>
		/// <param name="postId"></param>
		/// <param name="fileIds"></param>
		/// <returns></returns>
		public async Task<List<PostFile>> AddPostFiles(Guid postId, List<Guid> fileIds)
		{
			List<PostFile> result = new List<PostFile>();
			fileIds.ForEach(f =>
			{
				var postFileEntry = new PostFile(postId, f);
				_dbContext.Entry(postFileEntry).State = EntityState.Added;
				result.Add(postFileEntry);

			});

			var files = await _dbContext.Files.Where(f => fileIds.Contains(f.id)).ToListAsync();
			files.ForEach(file =>
			{
				file.entityOwnerId = postId;
				_dbContext.Entry(file).State = EntityState.Modified;
			});
			return result;
		}

		/// <summary>
		/// метод удаления связи между файлом и постом
		/// </summary>
		/// <param name="postId"></param>
		/// <param name="fileId"></param>
		/// <returns></returns>
		public async Task<PostFile> DeletePostFile(Guid postId, Guid fileId)
		{
			var postFileEntry = await _dbContext.PostFiles.Where(x => x.fileId == fileId && x.postId == postId).FirstOrDefaultAsync();
			await DeleteAsync(postFileEntry);
			return postFileEntry;
		}

		/// <summary>
		/// метод удаления файлов выбранного поста
		/// </summary>
		/// <param name="postId"></param>
		/// <returns></returns>
		public async Task<List<PostFile>> DeletePostFiles(Guid postId, List<Guid> postFilesIds, bool withCommit = false)
		{
			var postFiles = await _dbContext.PostFiles.Where(x => x.postId == postId 
				&& postFilesIds.Contains(x.fileId)).ToListAsync();
			postFiles.ForEach(file =>
			{
				_dbContext.Entry(file).State = EntityState.Deleted;
			});

			if (withCommit)
			{
				await _dbContext.SaveChangesAsync();
			}

			return postFiles;
		}
	}
}

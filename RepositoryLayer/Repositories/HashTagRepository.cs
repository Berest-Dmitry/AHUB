using DomainLayer.Data;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.IRepositories;
using RepositoryLayer.Repositories.Base;

namespace RepositoryLayer.Repositories
{
	/// <summary>
	///  репозиторий работы с хештегами
	/// </summary>
	public class HashTagRepository: Repository<HashTag>, IHashTagRepository
	{
		public HashTagRepository(AHUBContext dbContext) : base(dbContext) { }

		/// <summary>
		/// метод добавления хештега
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public async Task<HashTag> AddHasTagEntry(string content)
		{
			var hashTag = new HashTag(content);
			await AddAsync(hashTag);
			return hashTag;
		}

		/// <summary>
		/// метод получения списка всех хештегов
		/// </summary>
		/// <returns></returns>
		public async Task<List<HashTag>> GetAllTags(int skip, int take)
		{
			return await _dbContext.HashTags.Where(h => h.deletedAt == null).Skip(skip).Take(take).ToListAsync();
		}

		/// <summary>
		/// метод пометки записи о хештеге удаленной
		/// </summary>
		/// <param name="hashTagId"></param>
		/// <returns></returns>
		public async Task<HashTag> DeleteHashTag(Guid hashTagId)
		{
			var entry = await GetByIdAsync(hashTagId);
			entry.deletedAt = DateTime.UtcNow;
			_dbContext.Entry(entry).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
			return entry;
		}

		/// <summary>
		/// метод получения хештегов, чье содержимое похоже на введенный пользователем текст
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public async Task<List<HashTag>> GetHashTags(string content, int skip, int take)
		{
			return await _dbContext.HashTags
				.Where(h => h.content.Contains(content) && h.deletedAt == null)
				.Skip(skip).Take(take).ToListAsync();
		}
	}
}

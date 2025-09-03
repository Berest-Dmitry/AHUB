using DomainLayer.Data;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.IRepositories;
using RepositoryLayer.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Repositories
{
	/// <summary>
	/// репозиторий работы с жалобами
	/// </summary>
	public class AppealRepository: Repository<Appeal>, IAppealRepository
	{
		public AppealRepository(AHUBContext dbContext) : base(dbContext) { }

		/// <summary>
		/// метод создания записи о жалобе
		/// </summary>
		/// <param name="appeal"></param>
		/// <param name="propsToChange"></param>
		/// <returns></returns>
		public async Task<Appeal> CreateAppealEntry(Appeal appeal, List<string> propsToChange)
		{
			var appealEntry = new Appeal();
			FillEntityData(appeal, ref appealEntry, propsToChange);
			await AddAsync(appealEntry);
			return appealEntry;
		}

		/// <summary>
		/// метод получения всех жалоб на определенную тему
		/// </summary>
		/// <param name="entityId">код записи, к которой относятся жалобы</param>
		/// <returns></returns>
		public async Task<List<Appeal>> GetAllAppealsByEntity(Guid entityId)
		{
			return await _dbContext.Appeals.Where(a => a.appealEntityId == entityId)
				.OrderByDescending(a => a.dateTimeAdded).ToListAsync();
		}

		/// <summary>
		/// метод получения всех жалоб конкретного пользователя
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<List<Appeal>> GetAllAppealsByUser(Guid userId)
		{
			return await _dbContext.Appeals.Where(a => a.userId == userId)
				.OrderByDescending(a => a.dateTimeAdded).ToListAsync();
		}

		/// <summary>
		/// метод установки статуса жалобы
		/// </summary>
		/// <param name="appealId"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public async Task<Appeal> SetStatus(Guid appealId, short status)
		{
			var appeal = await GetByIdAsync(appealId);
			appeal.status = status;
			_dbContext.Entry(appeal).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
			return appeal;
		}
	}
}

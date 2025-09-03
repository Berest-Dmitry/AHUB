using DomainLayer.Models;
using RepositoryLayer.IRepositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.IRepositories
{
	/// <summary>
	/// интерфейс репозитория работы с жалобами
	/// </summary>
	public interface IAppealRepository: IRepository<Appeal>
	{

		Task<Appeal> CreateAppealEntry(Appeal appeal, List<string> propsToChange);

		Task<List<Appeal>> GetAllAppealsByEntity(Guid entityId);

		Task<List<Appeal>> GetAllAppealsByUser(Guid userId);

		Task<Appeal> SetStatus(Guid appealId, short status);
	}
}

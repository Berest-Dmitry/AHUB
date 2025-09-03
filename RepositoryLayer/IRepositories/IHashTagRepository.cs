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
	/// интерфейс репозитория работы с хештегами
	/// </summary>
	public interface IHashTagRepository: IRepository<HashTag>
	{
		Task<HashTag> AddHasTagEntry(string content);

		Task<List<HashTag>> GetAllTags(int skip, int take);

		Task<HashTag> DeleteHashTag(Guid hashTagId);

		Task<List<HashTag>> GetHashTags(string content, int skip, int take);
	}
}

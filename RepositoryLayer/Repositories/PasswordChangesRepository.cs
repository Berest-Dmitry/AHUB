using DomainLayer.Data;
using DomainLayer.Models;
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
	/// репозиторий сущности изменений паролей пользователей
	/// </summary>
	public class PasswordChangesRepository: Repository<PasswordChanges>, IPasswordChangesRepository
	{
		public PasswordChangesRepository(AHUBContext dbContext) : base(dbContext) { }
	}
}

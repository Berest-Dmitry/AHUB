using DomainLayer.Models;
using RepositoryLayer.IRepositories.Base;

namespace RepositoryLayer.IRepositories
{
	/// <summary>
	/// интерфейс репозитория сущности изменений паролей пользователей
	/// </summary>
	public interface IPasswordChangesRepository: IRepository<PasswordChanges>
	{
	}
}

using DomainLayer.Models;
using RepositoryLayer.IRepositories.Base;
using System.Linq.Expressions;

namespace RepositoryLayer.IRepositories
{
	/// <summary>
	/// интерфейс репозитория пользователей
	/// </summary>
	public interface IUsersRepository: IRepository<User>
	{
		Task<User> CreateOrUpdateUserEntry(User user, List<string> propsToChange);

		Task<User> GetUserById(Guid id);

		Task<User> DeleteUserEntry(Guid id);

		Task<List<User>> GetAllUsers(Expression<Func<User, bool>> predicate, int skip, int take);

		Task<bool> UpdatePassword(User user, string new_password, string salt);

		Task<User> GetUserByUserName(string username);

		Task<List<KeyValuePair<Guid, string>>> GetUserIdsWithNames();

		/// <summary>
		/// получение всей инф-ии по пользователям, включая связанные таблицы (посты, комментарии)
		/// </summary>
		/// <returns></returns>
		Task<List<User>> GetUsersWithLinkedData();
	}
}

using DomainLayer.Data;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.IRepositories;
using RepositoryLayer.Repositories.Base;
using System.Linq.Expressions;

namespace RepositoryLayer.Repositories
{
	/// <summary>
	/// репозиторий пользователей
	/// </summary>
	public class UsersRepository:  Repository<User>, IUsersRepository
	{
		public UsersRepository(AHUBContext dbContext) : base(dbContext) { }

		public async Task<User> CreateOrUpdateUserEntry(User user, List<string> propsToChange)
		{
			var existingUser = await GetByIdAsync(user.id);
			bool userToBeAdded = existingUser == null;

			if (userToBeAdded)
			{
				existingUser = new User();
			}
			FillEntityData(user, ref existingUser, propsToChange);

			if (userToBeAdded)
			{
				await AddAsync(existingUser);
			}
			else
			{
				_dbContext.Entry(existingUser).State = EntityState.Modified;
				await _dbContext.SaveChangesAsync();
			}
			return existingUser;

		}


		public async Task<User> GetUserById(Guid id)
		{
			return await GetByIdAsync(id);
		}

		public async Task<List<User>> GetAllUsers(Expression<Func<User, bool>> predicate, int skip, int take)
		{
			var usersList = await _dbContext.Users.Where(predicate).Skip(skip).Take(take).ToListAsync();
			return usersList;
		}

		/// <summary>
		/// метод пометки записи о пользователе удаленной
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<User> DeleteUserEntry(Guid id)
		{
			try
			{
				var existingUser = await GetByIdAsync(id);
				existingUser.deletedAt = DateTime.UtcNow;
				_dbContext.Entry(existingUser).State = EntityState.Modified;
				await _dbContext.SaveChangesAsync();
				return existingUser;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// метод обновления пароля пользователя
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="new_password"></param>
		/// <returns></returns>
		public async Task<bool> UpdatePassword(User user, string new_password, string salt)
		{
			try
			{
				user.hashedPassword = new_password;
				user.salt = salt;
				_dbContext.Entry(user).State = EntityState.Modified;
				await _dbContext.SaveChangesAsync();
				return true;
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// метод получения пользователя по логину
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public async Task<User> GetUserByUserName(string username)
		{
			var findedUser = await GetAsync(u => u.userName == username);
			return findedUser.FirstOrDefault();
		}

		/// <summary>
		/// метод получения списка ID пользователей с их именами
		/// </summary>
		/// <returns></returns>
		public async Task<List<KeyValuePair<Guid, string>>> GetUserIdsWithNames()
		{
			return await _dbContext.Users.Where(u => u.deletedAt == null)
				.Select(u => new KeyValuePair<Guid, string>(u.id, u.firstName + " " + u.lastName))
				.ToListAsync();
		}

        public async Task<List<User>> GetUsersWithLinkedData()
        {
            return await _dbContext.Users.Where(u => u.deletedAt == null)
				.Include(u => u.Posts)
				.Include(u => u.Comments)
				.ToListAsync();
        }
    }
}

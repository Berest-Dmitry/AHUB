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
	///  репозиторий сущности "UserRoles"
	/// </summary>
	public class UserRolesRepository: Repository<UserRoles>, IUserRolesRepository
	{
		public UserRolesRepository(AHUBContext dbContext) : base(dbContext) { }

		/// <summary>
		/// метод создания записи о роли пользователя в системе
		/// </summary>
		/// <param name="userRoles"></param>
		/// <returns></returns>
		public async Task<UserRoles> CreateUserRolesEntry(Guid userId, Guid roleId)
		{
			var userRoleEntry = new UserRoles(userId, roleId);
			await AddAsync(userRoleEntry);
			return userRoleEntry;

		}

		/// <summary>
		/// проверка, назначена ли уже выбранному пользователю эта роль
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleId"></param>
		/// <returns></returns>
		public async Task<bool> CheckIfUserHasRole(Guid userId, Guid roleId)
		{
			return await _dbContext.UserRoles
				.Where(x => x.userId == userId && x.roleId == roleId).CountAsync() > 0;
		}

		/// <summary>
		/// метод удаления записи о роли пользователя в системе
		/// </summary>
		/// <param name="userRolesId"></param>
		/// <returns></returns>
		public async Task<UserRoles> DeleteUserRolesEntry(Guid userId, Guid roleId)
		{
			var userRole = await _dbContext.UserRoles
				.Where(x => x.userId == userId && x.roleId == roleId).FirstOrDefaultAsync();
			_dbContext.Entry(userRole).State = EntityState.Deleted;
			await _dbContext.SaveChangesAsync();
			return userRole;
		}

		/// <summary>
		/// метод получения списка всех ролей текущего пользователя
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<List<Role>> GetListOfUserRoles(Guid userId)
		{
			var query = await (from ur in _dbContext.UserRoles
						 join r in _dbContext.Roles
						 on ur.roleId equals r.id
						 where ur.userId == userId
						 select r)
						 .ToListAsync();
			return query;
				
		}

		/// <summary>
		/// метод получения списка всех пользователей, у которых есть данная роль
		/// </summary>
		/// <param name="roleId"></param>
		/// <returns></returns>
		public async Task<List<Guid>> GetAllUsersWithCurrentRole(Guid roleId)
		{
			return await _dbContext.UserRoles.Where(ur => ur.roleId == roleId)
				.Select(ur => ur.userId).ToListAsync();
		}
	}
}

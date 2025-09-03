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
	/// репозиторий работы с ролями
	/// </summary>
	public class RoleRepository: Repository<Role>, IRoleRepository
	{
		public RoleRepository(AHUBContext dbContext) : base(dbContext) { }

		/// <summary>
		/// метод создания записи о роли
		/// </summary>
		/// <param name="role"></param>
		/// <param name="propsToChange"></param>
		/// <returns></returns>
		public async Task<Role> CreateRoleEntry(Role role, List<string> propsToChange)
		{
			var roleEntry = new Role();
			FillEntityData(role, ref roleEntry, propsToChange);
			await AddAsync(roleEntry);
			return roleEntry;
		}

		/// <summary>
		/// метод получения списка всех ролей в системе
		/// </summary>
		/// <returns></returns>
		public async Task<List<Role>> GetAllRoles()
		{
			return await GetAllAsync();
		}

		/// <summary>
		/// метод получения списка ролей по их ID
		/// </summary>
		/// <param name="roleIds">список кодов ролей</param>
		/// <returns></returns>
		public async Task<List<Role>> GetAllRoles(List<Guid> roleIds)
		{
			return await GetAsync(r => roleIds.Contains(r.id));
		}

		/// <summary>
		/// метод обновления записи о роли
		/// </summary>
		/// <param name="role"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public async Task<Role> UpdateRoleEntry(Role role, string roleName)
		{
			role.name = roleName;
			_dbContext.Entry(role).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
			return role;
		}

		/// <summary>
		/// метод пометки записи о роли как удаленной 
		/// </summary>
		/// <param name="roleId"></param>
		/// <returns></returns>
		public async Task<Role> DeleteRoleEntry(Guid roleId)
		{
			var roleToDelete = await GetByIdAsync(roleId);
			roleToDelete.deletedAt = DateTime.UtcNow;
			_dbContext.Entry(roleToDelete).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
			return roleToDelete;
		}

		/// <summary>
		/// метод получения кода роли по умолчанию
		/// </summary>
		/// <returns></returns>
		public async Task<Guid> GetDefaultUserRole()
		{
			return await _dbContext.Roles.Where(r => r.name == "User")
				.Select(x => x.id).FirstOrDefaultAsync();
		}
	}
}

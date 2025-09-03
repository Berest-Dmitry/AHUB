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
	/// интерфейс репозитория сущности "UserRoles"
	/// </summary>
	public interface IUserRolesRepository: IRepository<UserRoles>
	{
		Task<bool> CheckIfUserHasRole(Guid userId, Guid roleId);


		Task<UserRoles> CreateUserRolesEntry(Guid userId, Guid roleId);

		Task<UserRoles> DeleteUserRolesEntry(Guid userId, Guid roleId);

		Task<List<Role>> GetListOfUserRoles(Guid userId);

		Task<List<Guid>> GetAllUsersWithCurrentRole(Guid roleId);
	}
}

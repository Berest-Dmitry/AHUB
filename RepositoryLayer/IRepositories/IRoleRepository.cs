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
	/// интерфейс репозитория работы с ролями
	/// </summary>
	public interface IRoleRepository: IRepository<Role>
	{

		Task<Role> CreateRoleEntry(Role role, List<string> propsToChange);

		Task<List<Role>> GetAllRoles();

		Task<List<Role>> GetAllRoles(List<Guid> roleIds);

		Task<Role> UpdateRoleEntry(Role role, string roleName);

		Task<Role> DeleteRoleEntry(Guid roleId);

		Task<Guid> GetDefaultUserRole();
	}
}

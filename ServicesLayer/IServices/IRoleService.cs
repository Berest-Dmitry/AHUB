using ContractsLayer.Base;
using ContractsLayer.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.IServices
{
	/// <summary>
	/// интерфейс сервиса работы с ролями
	/// </summary>
	public interface IRoleService
	{

		Task<BaseResponseModel<RoleDto>> CreateRole(RoleDto roleDto);

		Task<BaseResponseModel<RoleDto>> UpdateRole(Guid roleId, string newRoleName);

		Task<BaseResponseModel<RoleDto>> DeleteRole(Guid roleId);

		Task<BaseResponseModel<UserRoleDto>> AttachRoleToUser(Guid userId, Guid roleId);

		Task<BaseResponseModel<UserRoleDto>> DetachRoleFromUser(Guid userId, Guid roleId);

		Task<BaseResponseModel<List<RoleDto>>> GetAllUserRoles(Guid userId);
	}
}

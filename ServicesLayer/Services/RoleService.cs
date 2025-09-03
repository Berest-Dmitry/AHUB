using ContractsLayer.Base;
using ContractsLayer.Common;
using ContractsLayer.Dtos;
using DomainLayer.Models;
using RepositoryLayer.IRepositories;
using ServicesLayer.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Services
{
	/// <summary>
	/// сервис работы с ролями
	/// </summary>
	public class RoleService: IRoleService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly IServiceManager _serviceManager;
		
		public RoleService(IRepositoryManager repositoryManager, IServiceManager serviceManager)
		{
			_repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
		}

		/// <summary>
		/// метод создания роли в системе;
		/// оступ только для администратора
		/// </summary>
		/// <param name="roleDto"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<RoleDto>> CreateRole(RoleDto roleDto)
		{
			try
			{
				var roleEntity = ObjectMapper.Mapper.Map<Role>(roleDto);
				var roleProps = ObjectManager<RoleDto>.GetListOfObjectPropertyNames(roleDto);
				var saveRes = await _repositoryManager._roleRepository.CreateRoleEntry(roleEntity, roleProps);

				if(saveRes != null)
				{
					return new BaseResponseModel<RoleDto>
					{
						Entity = ObjectMapper.Mapper.Map<RoleDto>(saveRes),
						Result = DefaultEnums.Result.ok
					};
				}
				else
				{
					return new BaseResponseModel<RoleDto>(new Exception("Failed to create role!"));
				}
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<RoleDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод обновления информации о роли
		/// доступ только для администратора
		/// </summary>
		/// <param name="roleId"></param>
		/// <param name="newRoleName"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<RoleDto>> UpdateRole(Guid roleId, string newRoleName)
		{
			try
			{
				var currentRole = await _repositoryManager._roleRepository.GetByIdAsync(roleId);

				var editRes = await _repositoryManager._roleRepository.UpdateRoleEntry(currentRole, newRoleName);
				if(editRes != null)
				{
					return new BaseResponseModel<RoleDto>
					{
						Entity = ObjectMapper.Mapper.Map<RoleDto>(editRes),
						Result = DefaultEnums.Result.ok
					};
				}
				else
				{
					return new BaseResponseModel<RoleDto>(
						new Exception("Failed to update role entry!")
						);
				}
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<RoleDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод удаления роли
		///  доступ только для администратора
		/// </summary>
		/// <param name="roleId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<RoleDto>> DeleteRole(Guid roleId)
		{
			try
			{
				var delRes = await _repositoryManager._roleRepository.DeleteRoleEntry(roleId);
				if(delRes != null)
				{
					return new BaseResponseModel<RoleDto>
					{
						Entity = ObjectMapper.Mapper.Map<RoleDto>(delRes),
						Result = DefaultEnums.Result.ok
					};
				}
				else
				{
					return new BaseResponseModel<RoleDto>(
						new Exception("Failed to remove role entry!")
						);
				}
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<RoleDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод добавления выбранному пользователю указанной роли;
		///  доступ только для администратора
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<UserRoleDto>> AttachRoleToUser(Guid userId, Guid roleId)
		{
			try
			{
				var isAssignedToUser = await _repositoryManager._userRolesRepository.CheckIfUserHasRole(userId, roleId);
				if (isAssignedToUser)
				{
					return new BaseResponseModel<UserRoleDto>(
						new Exception("User already has this role!")
						);
				}

				var createResult = await _repositoryManager._userRolesRepository.CreateUserRolesEntry(userId, roleId);
				if(createResult != null)
				{
					return new BaseResponseModel<UserRoleDto>
					{
						Entity = ObjectMapper.Mapper.Map<UserRoleDto>(createResult),
						Result = DefaultEnums.Result.ok
					};
				}
				else
				{
					return new BaseResponseModel<UserRoleDto>(
						new Exception("Failed to add role to chosen user")
						);
				}
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<UserRoleDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод лишения пользователя выбранной роли;
		///  доступ только для администратора
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<UserRoleDto>> DetachRoleFromUser(Guid userId, Guid roleId)
		{
			try
			{
				var delResult = await _repositoryManager._userRolesRepository.DeleteUserRolesEntry(userId, roleId);
				if(delResult != null)
				{
					return new BaseResponseModel<UserRoleDto>
					{
						Entity = ObjectMapper.Mapper.Map<UserRoleDto>(delResult),
						Result = DefaultEnums.Result.ok
					};
				}
				else
				{
					return new BaseResponseModel<UserRoleDto>(
						new Exception("Failed to remove role from chosen user")
						);
				}
			}
			catch(Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<UserRoleDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод получения списка всех ролей пользователя
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<RoleDto>>> GetAllUserRoles(Guid userId)
		{
			try
			{
				var userRoles = await _repositoryManager._userRolesRepository.GetListOfUserRoles(userId);
				if(userRoles?.Count == 0)
				{
					return new BaseResponseModel<List<RoleDto>>(
						new Exception("An error occurred while getting info about user's roles")
					);

				}

				return new BaseResponseModel<List<RoleDto>>
				{
					Result = DefaultEnums.Result.ok,
					Entity = ObjectMapper.Mapper.Map<List<RoleDto>>(userRoles)	
				};
			}
			catch (Exception ex)
			{
				return new BaseResponseModel<List<RoleDto>>(ex);
			}
		}
	}
}

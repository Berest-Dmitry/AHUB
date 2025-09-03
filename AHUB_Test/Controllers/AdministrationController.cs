using ContractsLayer.Base;
using ContractsLayer.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicesLayer.IServices;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace AHUB_Test.Controllers
{
	[Route("api/administration")]
	[ApiController]
	public class AdministrationController : ControllerBase
	{
		private readonly IServiceManager _serviceManager;

		public AdministrationController(IServiceManager serviceManager)
		{
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
		}

		/// <summary>
		/// взятие жалобы в работу
		/// </summary>
		/// <param name="appealId"></param>
		/// <returns></returns>
		/// <response code="200"> Appeal has been taken to work </response>
		/// <response code="401"> Request unauthorized </response>
		/// <response code="403"> Not enough rights to perform operation </response>
		[HttpPatch]
		[Route("take-to-work")]
		[Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<AppealDto>))]
		[SwaggerResponse((int)HttpStatusCode.Forbidden, "not enough rights to perform operation")]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> TakeAppealToWork(Guid appealId)
		{
			var result = await _serviceManager._appealService.TakeAppealToWork(appealId);
			return new JsonResult(result);
		}

		/// <summary>
		/// отклонение жалобы
		/// </summary>
		/// <param name="appealId"></param>
		/// <returns></returns>
		/// <response code="200"> Appeal has been denied </response>
		/// <response code="401"> Request unauthorized </response>
		/// <response code="403"> Not enough rights to perform operation </response>
		[HttpPatch]
		[Route("decline")]
		[Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<AppealDto>))]
		[SwaggerResponse((int)HttpStatusCode.Forbidden, "not enough rights to perform operation")]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> DenyAppeal(Guid appealId)
		{
			var result = await _serviceManager._appealService.DenyAppeal(appealId);
			return new JsonResult(result);
		}

		/// <summary>
		/// рассмотрение жалобы
		/// </summary>
		/// <param name="appealId"></param>
		/// <returns></returns>
		/// <response code="200"> Appeal has been considered </response>
		/// <response code="401"> Request unauthorized </response>
		/// <response code="403"> Not enough rights to perform operation </response>
		[HttpPatch]
		[Route("ready")]
		[Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<AppealDto>))]
		[SwaggerResponse((int)HttpStatusCode.Forbidden, "not enough rights to perform operation")]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> ConsiderAppeal(Guid appealId)
		{
			var result = await _serviceManager._appealService.ConsiderAppeal(appealId);
			return new JsonResult(result);
		}

		/// <summary>
		/// создание роли в системе
		/// </summary>
		/// <param name="roleDto"></param>
		/// <returns></returns>
		/// <response code="200"> Role created </response>
		/// <response code="401"> Request unauthorized </response>
		/// <response code="403"> Not enough rights to perform operation </response>
		[HttpPost]
		[Route("roles")]
		[Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<RoleDto>))]
		[SwaggerResponse((int)HttpStatusCode.Forbidden, "not enough rights to perform operation")]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> CreateRole([FromBody] RoleDto roleDto)
		{
			var roleCreateRes = await _serviceManager._roleService.CreateRole(roleDto);
			return new JsonResult(roleCreateRes);
		}

		/// <summary>
		/// обновление информации о роли в системе
		/// </summary>
		/// <param name="roleName"></param>
		/// <param name="roleId"></param>
		/// <returns></returns>
		/// <response code="200"> Role details updated </response>
		/// <response code="401"> Request unauthorized </response>
		/// <response code="403"> Not enough rights to perform operation </response>
		[HttpPatch]
		[Route("roles")]
		[Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<RoleDto>))]
		[SwaggerResponse((int)HttpStatusCode.Forbidden, "not enough rights to perform operation")]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> UpdateRole(string roleName, Guid roleId)
		{
			var editRoleRes = await _serviceManager._roleService.UpdateRole(roleId, roleName);
			return new JsonResult(editRoleRes);
		}

		/// <summary>
		/// удаление роли
		/// </summary>
		/// <param name="roleId"></param>
		/// <returns></returns>
		/// <response code="200"> Role removed from system </response>
		/// <response code="401"> Request unauthorized </response>
		/// <response code="403"> Not enough rights to perform operation </response>
		[HttpDelete]
		[Route("roles")]
		[Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<RoleDto>))]
		[SwaggerResponse((int)HttpStatusCode.Forbidden, "not enough rights to perform operation")]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> DeleteRole(Guid roleId)
		{
			var removeRoleRes = await _serviceManager._roleService.DeleteRole(roleId);
			return new JsonResult(removeRoleRes);
		}

		/// <summary>
		/// добавление роли пользователю
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleId"></param>
		/// <returns></returns>
		/// <response code="200"> Role has been assigned to user </response>
		/// <response code="401"> Request unauthorized </response>
		/// <response code="403"> Not enough rights to perform operation </response>
		[HttpPost]
		[Route("user-roles")]
		[Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<UserRoleDto>))]
		[SwaggerResponse((int)HttpStatusCode.Forbidden, "not enough rights to perform operation")]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> AttachRoleToUser(Guid userId, Guid roleId)
		{
			var attachRes = await _serviceManager._roleService.AttachRoleToUser(userId, roleId);
			return new JsonResult(attachRes);
		}

		/// <summary>
		/// удаление роли пользователя
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleId"></param>
		/// <returns></returns>
		/// <response code="200"> Role has been removed from user </response>
		/// <response code="401"> Request unauthorized </response>
		/// <response code="403"> Not enough rights to perform operation </response>
		[HttpDelete]
		[Route("user-roles")]
		[Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<UserRoleDto>))]
		[SwaggerResponse((int)HttpStatusCode.Forbidden, "not enough rights to perform operation")]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> DetachRoleFromUser(Guid userId, Guid roleId)
		{
			var detachRes = await _serviceManager._roleService.DetachRoleFromUser(userId, roleId);
			return new JsonResult(detachRes);
		}

		/// <summary>
		/// заполнение индекса постов в Elastic
		/// </summary>
		/// <returns></returns>
		/// <response code="200"> Fill Elastic "posts" index </response>
		/// <response code="401"> Request unauthorized </response>
		/// <response code="403"> Not enough rights to perform operation </response>
		[HttpPost]
		[Route("elastic-posts")]
		[Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<BaseModel>))]
		[SwaggerResponse((int)HttpStatusCode.Forbidden, "not enough rights to perform operation")]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> FillElasticPostIndex()
		{
			var fillRes = await _serviceManager._elasticPostService.BulkInsertPosts();
			if(fillRes?.Result == ContractsLayer.Common.DefaultEnums.Result.error)
			{
				return StatusCode(207, fillRes);
			}
			else return new JsonResult(fillRes);
		}

		/// <summary>
		///  очистка индекса постов в Elastic
		/// </summary>
		/// <returns></returns>
		/// <response code="200"> Clear Elastic "posts" index </response>
		/// <response code="401"> Request unauthorized </response>
		/// <response code="403"> Not enough rights to perform operation </response>
		[HttpDelete]
		[Route("elastic-posts")]
		[Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<BaseModel>))]
		[SwaggerResponse((int)HttpStatusCode.Forbidden, "not enough rights to perform operation")]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> ClearElasticPostIndex()
		{
			var delRes = await _serviceManager._elasticPostService.DeleteAllPosts();
			if(delRes?.Result == ContractsLayer.Common.DefaultEnums.Result.error)
			{
				return StatusCode(500, delRes?.Error);
			}
			return new JsonResult(delRes);
		}
	}
}

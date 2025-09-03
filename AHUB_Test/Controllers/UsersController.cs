using ContractsLayer.Base;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesLayer.IServices;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace AHUB_Test.Controllers
{
    [Route("api/users")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IServiceManager _serviceManager;
		private readonly IConfiguration _configuration;

		public UsersController(IServiceManager serviceManager, IConfiguration config)
		{
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
			_configuration = config ?? throw new ArgumentNullException(nameof(config));
		}

		/// <summary>
		/// обновление информации о пользователе
		/// </summary>
		/// <param name="userInfo"></param>
		/// <returns></returns>
		/// <response code="200"> Updated user info </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpPost]
		[Route("update-user")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<UserDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto userInfo)
		{
			var res = await _serviceManager._userService.UpdateUserData(userInfo);
			return new JsonResult(res);
		}

		/// <summary>
		/// удаление пользователя
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		/// <response code="200"> Deleted user </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpDelete]
		[Route("remove-user")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<UserDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> RemoveUser(Guid userId)
		{
			var res = await _serviceManager._userService.RemoveUser(userId);
			return new JsonResult(res); 
		}
	}
}

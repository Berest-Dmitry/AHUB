using ContractsLayer.Base;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicesLayer.IServices;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace AHUB_Test.Controllers
{
	[Route("api/appeals")]
	[ApiController]
	public class AppealController : ControllerBase
	{
		private readonly IServiceManager _serviceManager;

		public AppealController(IServiceManager serviceManager)
		{
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
		}

		/// <summary>
		/// написание жалобы
		/// </summary>
		/// <param name="appealDto"></param>
		/// <returns></returns>
		/// <response code="200"> Appeal has been written </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpPost]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<AppealDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> WriteAppeal([FromBody] AppealCreateDto appealDto)
		{
			var result = await _serviceManager._appealService.WriteAppeal(appealDto);
			return new JsonResult(result);
		}

		/// <summary>
		/// получение списка всех жалоб пользователя
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		/// <response code="200"> User's appeals </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpGet]
		[Route("users/{userId}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<List<AppealDto>>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> GetAllUserAppeals(Guid userId)
		{
			var result = await _serviceManager._appealService.GetAllUserAppeals(userId);
			return new JsonResult(result);
		}

		/// <summary>
		/// получение списка всех жалоб по определенному поводу
		/// </summary>
		/// <param name="subjectId"></param>
		/// <returns></returns>
		/// <response code="200"> Appeals by subject </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpGet]
		[Route("subjects/{subjectId}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<List<AppealDto>>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> GetAppealsBySubject(Guid subjectId)
		{
			var result = await _serviceManager._appealService.GetAppealsBySubject(subjectId);
			return new JsonResult(result);
		}

		
	}
}

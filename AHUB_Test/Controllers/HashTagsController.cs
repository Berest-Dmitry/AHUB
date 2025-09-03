using ContractsLayer.Base;
using ContractsLayer.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesLayer.IServices;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace AHUB_Test.Controllers
{
	[Route("api/hashtags")]
	[ApiController]
	public class HashTagsController : ControllerBase
	{
		private readonly IServiceManager _serviceManager;
		private readonly IConfiguration _configuration;

		public HashTagsController(IServiceManager serviceManager, IConfiguration configuration)
		{
			_serviceManager = serviceManager ?? throw new NullReferenceException(nameof(serviceManager));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		/// <summary>
		/// список хеш теговб соответствующих поиску
		/// </summary>
		/// <param name="content"></param>
		/// <param name="skip"></param>
		/// <param name="take"></param>
		/// <returns></returns>
		/// <response code="200"> List of hash tags </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpGet]
		[Route("search")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<List<HashTagDto>>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> GetHashTagsByContent(string content, int skip, int take)
		{
			var res = await _serviceManager._hashTagService.GetHashTagsByContent(content, skip, take);
			return new JsonResult(res);
		}

		/// <summary>
		/// создание хеш тега
		/// </summary>
		/// <param name="hashTag"></param>
		/// <returns></returns>
		/// <response code="200"> Created hash tag </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpPost]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<HashTagDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> CreateHashTag(string hashTag)
		{

			var res = await _serviceManager._hashTagService.CreateHashTag(hashTag);
			return new JsonResult(res);
 		}

		/// <summary>
		/// удаление хеш тега
		/// </summary>
		/// <param name="hasTagId"></param>
		/// <returns></returns>
		/// <response code="200"> Removed hash tag </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpDelete("{hashTagId}")]
		[Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<HashTagDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		
		public async Task<IActionResult> RemoveHashTag(Guid hasTagId)
		{
			var res = await _serviceManager._hashTagService.RemoveHashTag(hasTagId);
			return new JsonResult(res); 
		}
	}
}

using ContractsLayer.Base;
using ContractsLayer.Common;
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
	[Route("api/posts")]
	[ApiController]
	public class PostsController : ControllerBase
	{
		private readonly IServiceManager _serviceManager;
		private readonly IConfiguration _configuration;

		public PostsController(IServiceManager serviceManager, IConfiguration configuration)
		{
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		/// <summary>
		/// создание поста
		/// </summary>
		/// <param name="postWithFilesDto"></param>
		/// <returns></returns>
		/// <response code="200"> Created post </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpPost]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<PostDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> CreatePost([FromBody] PostWithFilesCreate postWithFilesDto)
		{
			var result = await _serviceManager._postsService.CreatePost(postWithFilesDto, HttpContext);
			return new JsonResult(result);
		}

		/// <summary>
		/// получение списка ссылок на пользователей сайта
		/// </summary>
		/// <returns></returns>
		/// <response code="200"> User links </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpGet]
		[Route("user-links")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<List<UserLinkDto>>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> GetUserLinks()
		{

			var res = await _serviceManager._postsService.GetUserLinksForSelect();
			return new JsonResult(res);
		}

		/// <summary>
		/// редактирование поста
		/// </summary>
		/// <param name="postWithFilesUpdate"></param>
		/// <param name="postId"></param>
		/// <returns></returns>
		/// <response code="200"> Edited post </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpPut("{postId}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<PostDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> EditPost([FromBody] PostWithFilesUpdate postWithFilesUpdate, Guid postId)
		{
			var result = await _serviceManager._postsService.UpdatePost(postWithFilesUpdate, postId, HttpContext);
			return new JsonResult(result);
		}

		/// <summary>
		/// удаление поста
		/// </summary>
		/// <param name="postId"></param>
		/// <returns></returns>
		/// <response code="200"> Deleted post </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpDelete("{postId}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<PostDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> DeletePost(Guid postId)
		{
			var res = await _serviceManager._postsService.DeletePost(postId, HttpContext);
			return new JsonResult(res);
		}

		/// <summary>
		/// получение поста
		/// </summary>
		/// <param name="postId"></param>
		/// <returns></returns>
		/// <response code="200"> Post entry </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpGet("{postId}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<PostDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> GetPost(Guid postId)
		{
			var res = await _serviceManager._postsService.GetPost(postId);
			return new JsonResult(res);
		}

		/// <summary>
		/// получение списка публикаций выбранного пользователя
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		/// <response code="200"> Posts of chosen user </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpGet]
		[Route("users/{userId}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<List<PostDto>>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> GetPostsOfCurrentUser(Guid userId)
		{
			var res = await _serviceManager._postsService.GetPostsOfCurrentUser(userId);
			return new JsonResult(res);
		}

		/// <summary>
		/// удаление файла из поста
		/// </summary>
		/// <param name="postId"></param>
		/// <param name="fileId"></param>
		/// <returns></returns>
		/// <response code="200"> Detached file entry </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpDelete]
		[Route("detach-file")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<PostFileDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> DetachFile(Guid postId, Guid fileId)
		{
			var res = await _serviceManager._postFilesService.DetachFileFromPost(postId, fileId);
			return new JsonResult(res);
		}

		/// <summary>
		/// получение прикрепленных файлов
		/// </summary>
		/// <param name="postId"></param>
		/// <returns></returns>
		/// <response code="200"> List of attached files' IDs </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpGet]
		[Route("files-list/{postId}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<List<Guid>>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> GetFilesAttachedToPost(Guid postId)
		{
			var res = await _serviceManager._postFilesService.GetFilesAttachedToPost(postId);
			return new JsonResult(res);
		}

		/// <summary>
		/// поиск постов при помощи Elastic
		/// </summary>
		/// <param name="term"></param>
		/// <param name="skip"></param>
		/// <param name="take"></param>
		/// <returns></returns>
		/// <response code="200"> Posts found </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpGet]
		[Route("search")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<List<PostDto>>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> SearchPostsElastic(string term, int skip, int take)
		{
			var res = await _serviceManager._postsService.SearchPostsElastic(term, skip, take);
			return new JsonResult(res);
		}
	}
}

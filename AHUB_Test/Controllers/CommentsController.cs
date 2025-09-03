using ContractsLayer.Base;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using ContractsLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicesLayer.IServices;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace AHUB_Test.Controllers
{
	[Route("api/comments")]
	[ApiController]
	public class CommentsController : ControllerBase
	{
		private readonly IServiceManager _serviceManager;

		public CommentsController(IServiceManager serviceManager)
		{
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
		}

		/// <summary>
		/// отправка комментария к посту
		/// </summary>
		/// <param name="comment"></param>
		/// <returns></returns>
		/// <response code="200"> Created comment </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpPost]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<CommentDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> CommentOnPost([FromBody] CommentCreateDto comment)
		{
			var result = await _serviceManager._commentService.CommentOnPost(comment);
			return new JsonResult(result);
		}

		/// <summary>
		/// отправка ответа на комментарий
		/// </summary>
		/// <param name="comment"></param>
		/// <returns></returns>
		/// <response code="200"> Reply to comment </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpPost]
		[Route("reply")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<CommentDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> ReplyToComment([FromBody] CommentCreateDto comment)
		{
			var result = await _serviceManager._commentService.ReplyToComment(comment);
			return new JsonResult(result);
		}

		/// <summary>
		/// редактирование комментария
		/// </summary>
		/// <param name="commentText"></param>
		/// <param name="commentId"></param>
		/// <returns></returns>
		/// <response code="200"> Comment  edited </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpPatch("{commentId}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<CommentDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> EditComment(string commentText, Guid commentId)
		{
			var result = await _serviceManager._commentService.EditComment(commentText, commentId, HttpContext);
			return new JsonResult(result);
		}

		/// <summary>
		/// загрузка комментариев к посту
		/// </summary>
		/// <param name="postId"></param>
		/// <param name="skip"></param>
		/// <param name="take"></param>
		/// <returns></returns>
		/// <response code="200"> List of comments related to post </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpGet("{postId}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<List<CommentDto>>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> GetCommentsOnPost(Guid postId, int skip, int take)
		{
			var result = await _serviceManager._commentService.GetCommentsOnPost(postId, skip, take);
			return new JsonResult(result);
		}

		/// <summary>
		/// получение списка дочерних комментариев
		/// </summary>
		/// <param name="parentId"></param>
		/// <param name="skip"></param>
		/// <param name="take"></param>
		/// <returns></returns>
		/// <response code="200"> List of child comments </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpGet]
		[Route("by-parent/{parentId}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<List<CommentDto>>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> GetChildComments(Guid parentId, int skip, int take)
		{
			var result = await _serviceManager._commentService.LoadChildComments(parentId, skip, take);
			return new JsonResult(result);
		}

		/// <summary>
		/// получение общего кол-ва дочерних комментариев к текущему
		/// </summary>
		/// <param name="parentIds"></param>
		/// <returns></returns>
		/// <response code="200"> Total count of child cmments </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpPost]
		[Route("children-count")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<List<CommentCountModel>>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> GetTotalChildrenCount([FromBody] List<Guid> parentIds)
		{
			var result = await _serviceManager._commentService.GetTotalChildrenCount(parentIds);
			return new JsonResult(result);
		}

		/// <summary>
		/// удаление комментария к посту
		/// </summary>
		/// <param name="commentId"></param>
		/// <returns></returns>
		/// <response code="200"> Deleted comment </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpDelete("{commentId}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<CommentDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> DeleteComment(Guid commentId)
		{
			var result = await _serviceManager._commentService.DeleteComment(commentId, HttpContext);
			return new JsonResult(result);
		}

		/// <summary>
		/// восстановление комментария
		/// </summary>
		/// <param name="commentId"></param>
		/// <returns></returns>
		/// <response code="200"> Restored comment </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpPost]
		[Route("restore/{commentId}")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<CommentDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> RestoreComment(Guid commentId)
		{
			var result = await _serviceManager._commentService.RestoreComment(commentId, HttpContext);
			return new JsonResult(result);
		}
	}
}

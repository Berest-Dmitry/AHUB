using ContractsLayer.Base;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using ContractsLayer.Models;
using Microsoft.AspNetCore.Http;


namespace ServicesLayer.IServices
{
	/// <summary>
	/// интерфейс сервиса работы с комментариями
	/// </summary>
	public interface ICommentService
	{
		Task<BaseResponseModel<CommentDto>> CommentOnPost(CommentCreateDto commentDto);

		Task<BaseResponseModel<CommentDto>> EditComment(string commentText, Guid commentId, HttpContext httpContext);

		Task<BaseResponseModel<CommentDto>> ReplyToComment(CommentCreateDto commentDto);

		Task<BaseResponseModel<List<CommentDto>>> GetCommentsOnPost(Guid postId, int skip, int take);

		Task<BaseResponseModel<List<CommentDto>>> LoadChildComments(Guid parentId, int skip, int take);

		Task<BaseResponseModel<List<CommentCountModel>>> GetTotalChildrenCount(List<Guid> parentIds);

		Task<BaseResponseModel<CommentDto>> DeleteComment(Guid commentId, HttpContext httpContext);

		Task<BaseResponseModel<CommentDto>> RestoreComment(Guid commentId, HttpContext httpContext);
	}
}

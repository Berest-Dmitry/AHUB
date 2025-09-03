using ContractsLayer.Base;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.IServices
{
	/// <summary>
	/// интерфейс сервиса работы с постами
	/// </summary>
	public interface IPostsService
	{
		Task<BaseResponseModel<PostDto>> CreatePost(PostWithFilesCreate postWithFilesDto, HttpContext context);

		Task<List<UserLinkDto>> GetUserLinksForSelect();

		Task<BaseResponseModel<PostDto>> GetPost(Guid postId);

		Task<BaseResponseModel<List<PostDto>>> GetPostsOfCurrentUser(Guid userId);

		Task<BaseResponseModel<PostDto>> UpdatePost(PostWithFilesUpdate postUpdateDto, Guid postId, HttpContext context);

		Task<BaseResponseModel<PostDto>> DeletePost(Guid postId, HttpContext context);

		Task<BaseResponseModel<List<PostDto>>> GetPostsList(List<Guid> postIds);

		Task<BaseResponseModel<List<PostDto>>> SearchPostsElastic(string term, int skip, int take);
	}
}

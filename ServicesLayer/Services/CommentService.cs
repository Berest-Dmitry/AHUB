using ContractsLayer.Base;
using ContractsLayer.Common;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using ContractsLayer.Models;
using DomainLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.IRepositories;
using ServicesLayer.IServices;
using System.Net.Http;


namespace ServicesLayer.Services
{
	/// <summary>
	/// сервис работы с комментариями
	/// </summary>
	public class CommentService: ICommentService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly IConfiguration _configuration;
		private readonly IServiceManager _serviceManager;

		public CommentService(IRepositoryManager repositoryManager, IConfiguration configuration, IServiceManager serviceManager)
		{
			_repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
		}

		/// <summary>
		/// метод сохранения комментария к посту
		/// </summary>
		/// <param name="commentDto"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<CommentDto>> CommentOnPost(CommentCreateDto commentDto)
		{
			try
			{
				var currentUser = await _serviceManager._userService.GetCurrentUser(commentDto.userId);
				if (currentUser?.Error != null)
				{
					return new BaseResponseModel<CommentDto>(currentUser.Error);
				}

				var commentEntry = ObjectMapper.Mapper.Map<Comment>(commentDto);
				var commentProps = ObjectManager<CommentDto>.GetListOfObjectPropertyNames(new CommentDto());
				var saveRes = await _repositoryManager._commentRepository.CreateCommentEntry(commentEntry, commentProps);

				if(saveRes != null)
				{
					return new BaseResponseModel<CommentDto>
					{
						Entity = ObjectMapper.Mapper.Map<CommentDto>(saveRes),
						Result = DefaultEnums.Result.ok
					};
				}
				else
				{
					return new BaseResponseModel<CommentDto>(new Exception("Failed to save comment!"));
				}
			}
			catch(Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<CommentDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод редактирования комментария пользователя
		/// </summary>
		/// <param name="commentText">текст комментария</param>
		/// <param name="commentId">код существующей записи о комментарии</param>
		/// <param name="userName">логин пользователя</param>
		/// <returns></returns>
		public async Task<BaseResponseModel<CommentDto>> EditComment(string commentText, Guid commentId, HttpContext httpContext)
		{
			try
			{
				var userNameResult = await _serviceManager._userService.GetUserNameFromToken(httpContext, _configuration);

				var currentComment = await _repositoryManager._commentRepository.GetByIdAsync(commentId);
				var isAllowedToEdit = await CheckIfUserCanEditComment(currentComment.userId, userNameResult?.Entity);
				if(isAllowedToEdit?.Result != DefaultEnums.Result.ok)
				{
					return new BaseResponseModel<CommentDto>(isAllowedToEdit?.Error);
				}

				var editRes = await _repositoryManager._commentRepository.UpdateCommentEntry(currentComment, commentText);
				if (editRes != null)
				{
					return new BaseResponseModel<CommentDto>
					{
						Entity = ObjectMapper.Mapper.Map<CommentDto>(editRes),
						Result = DefaultEnums.Result.ok
					};
				}
				else return new BaseResponseModel<CommentDto>(new Exception("An error occurred while changing comment entry"));
			}
			catch(Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<CommentDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод сохранения ответа на текущий комментарий
		/// </summary>
		/// <param name="commentDto"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<CommentDto>> ReplyToComment(CommentCreateDto commentDto)
		{
			try
			{
				var currentUser = await _serviceManager._userService.GetCurrentUser(commentDto.userId);
				if (currentUser?.Error != null)
				{
					return new BaseResponseModel<CommentDto>(currentUser.Error);
				}

				var parentOfThisComment = await _repositoryManager._commentRepository.GetByIdAsync(commentDto.parentId ?? Guid.Empty);
				if (parentOfThisComment?.parentId != null)
				{
					return new BaseResponseModel<CommentDto>(new Exception("One can reply only to top-level comments!"));
				}

				var commentEntity = ObjectMapper.Mapper.Map<Comment>(commentDto);
				var commentProps = ObjectManager<CommentDto>.GetListOfObjectPropertyNames(new CommentDto());

				var saveRes = await _repositoryManager._commentRepository.CreateCommentEntry(commentEntity, commentProps);
				if (saveRes != null)
				{
					return new BaseResponseModel<CommentDto>
					{
						Entity = ObjectMapper.Mapper.Map<CommentDto>(saveRes),
						Result = DefaultEnums.Result.ok
					};
				}
				else return new BaseResponseModel<CommentDto>(new Exception("Failed to save reply to current comment"));
			}
			catch(Exception ex) { 
				return BaseModelUtilities<BaseResponseModel<CommentDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// проверка на наличие прав у пользователя на редактирование/ удаление комментария
		/// </summary>
		/// <param name="commentAuthorId">код автора комментария</param>
		/// <param name="userName">логин текущего пользователя</param>
		/// <returns></returns>
		private async Task<BaseModel> CheckIfUserCanEditComment(Guid commentAuthorId, string userName)
		{
			try
			{
				var currentUser = await _serviceManager._userService.GetCurrentUser(userName);
				if (currentUser?.Error != null)
				{
					return new BaseResponseModel<CommentDto>(currentUser.Error);
				}

				if (currentUser?.Entity.id != commentAuthorId)
				{
					return new BaseResponseModel<CommentDto>(new Exception("You're not allowed to edit this comment!"));
				}
				return new BaseModel();
			}
			catch(Exception ex)
			{
				return BaseModelUtilities<BaseModel>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод получения списка комментариев к посту с пагинацией
		/// </summary>
		/// <param name="postId">код поста</param>
		/// <param name="skip">пропустить</param>
		/// <param name="take">взять</param>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<CommentDto>>> GetCommentsOnPost(Guid postId, int skip, int take)
		{
			try
			{
				var result = await _repositoryManager._commentRepository.GetCommentsAttachedToPost(postId, skip, take);
				if (result != null && result.Count > 0)
				{
					result.ForEach(c =>
					{
						if(c.deletedAt != null)
						{
							c.content = null;
						}
					});
					return new BaseResponseModel<List<CommentDto>>
					{
						Entity = ObjectMapper.Mapper.Map<List<CommentDto>>(result),
						Result = DefaultEnums.Result.ok
					};
				}
				else return new BaseResponseModel<List<CommentDto>>(
					new Exception("This post has no comments yet")
					);
			}
			catch(Exception ex)
			{
				return new BaseResponseModel<List<CommentDto>>(ex);
			}
		}

		/// <summary>
		/// метод получения набора пар типа ключ-значение;
		/// ключ - код родителского комментария,
		/// значение - кол-во комментариев, являющихся дочерними для текущего 
		/// </summary>
		/// <param name="parentIds"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<CommentCountModel>>> GetTotalChildrenCount(List<Guid> parentIds)
		{
			try
			{
				var result = new List<CommentCountModel>();
				var data = await _repositoryManager._commentRepository.GetTotalCountOfChildComments(parentIds);
				data.ForEach(item =>
				{
					var commentWithChildren = new CommentCountModel(item.Key, item.Value);
					result.Add(commentWithChildren);
				});

				return new BaseResponseModel<List<CommentCountModel>>(result);
			}
			catch (Exception ex)
			{
				return new BaseResponseModel<List<CommentCountModel>>(ex);
			}
		}

		/// <summary>
		/// метод получения списка ответов на комментарий с пагинеацией
		/// </summary>
		/// <param name="parentId">код родительского комментария</param>
		/// <param name="skip">пропустить</param>
		/// <param name="take">взять</param>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<CommentDto>>> LoadChildComments(Guid parentId, int skip, int take)
		{
			try
			{
				var result = await _repositoryManager._commentRepository.GetListOfChildComments(parentId, skip, take);
				if (result != null && result.Count > 0)
				{
					result.ForEach(c =>
					{
						if(c.deletedAt != null)
						{
							c.content = null;
						}
					});
					return new BaseResponseModel<List<CommentDto>>
					{
						Entity = ObjectMapper.Mapper.Map<List<CommentDto>>(result),
						Result = DefaultEnums.Result.ok
					};
				}
				else return new BaseResponseModel<List<CommentDto>>(
					new Exception("This comment has no replies yet")
					);
			}
			catch(Exception ex)
			{
				return new BaseResponseModel<List<CommentDto>>(ex);
			}
		}

		/// <summary>
		/// метод пометки текущего комментария удаленным
		/// </summary>
		/// <param name="commentId">код комментария</param>
		/// <param name="userName">логин пользователя</param>
		/// <returns></returns>
		public async Task<BaseResponseModel<CommentDto>> DeleteComment(Guid commentId, HttpContext httpContext)
		{
			try
			{
				var userNameResult = await _serviceManager._userService.GetUserNameFromToken(httpContext, _configuration);

				var currentComment = await _repositoryManager._commentRepository.GetByIdAsync(commentId);
				var isAllowedToEdit = await CheckIfUserCanEditComment(currentComment.userId, userNameResult?.Entity);
				if (isAllowedToEdit?.Result != DefaultEnums.Result.ok)
				{
					return new BaseResponseModel<CommentDto>(isAllowedToEdit?.Error);
				}

				if(currentComment.deletedAt != null)
				{
					return new BaseResponseModel<CommentDto>(
						new Exception("This comment has already been removed!")
						);
				}

				var delResult = await _repositoryManager._commentRepository.DeleteOrRestoreCommentEntry(commentId);
				if (delResult != null)
				{
					return new BaseResponseModel<CommentDto>
					{
						Entity = ObjectMapper.Mapper.Map<CommentDto>(delResult),
						Result = DefaultEnums.Result.ok
					};
				}
				else return new BaseResponseModel<CommentDto>(
					new Exception("Failed to remove current comment")
					);
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<CommentDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод восстановления удаленного комментария
		/// </summary>
		/// <param name="commentId">код комментария</param>
		/// <param name="userName">логин пользователя</param>
		/// <returns></returns>
		public async Task<BaseResponseModel<CommentDto>> RestoreComment(Guid commentId, HttpContext httpContext)
		{
			try
			{
				var userNameResult = await _serviceManager._userService.GetUserNameFromToken(httpContext, _configuration);

				var currentComment = await _repositoryManager._commentRepository.GetByIdAsync(commentId);
				var isAllowedToEdit = await CheckIfUserCanEditComment(currentComment.userId, userNameResult?.Entity);
				if (isAllowedToEdit?.Result != DefaultEnums.Result.ok)
				{
					return new BaseResponseModel<CommentDto>(isAllowedToEdit?.Error);
				}

				if(currentComment?.deletedAt == null)
				{
					return new BaseResponseModel<CommentDto>(
						new Exception("This comment is already restored or hasn't been deleted yet!")
						);
				}

				if(DateTime.UtcNow > currentComment?.deletedAt.Value.AddMinutes(10))
				{
					return new BaseResponseModel<CommentDto>(
						new Exception("Time to restore this comment has expired!")
						);
				}

				var restoreRes = await _repositoryManager._commentRepository.DeleteOrRestoreCommentEntry(commentId, true);
				if(restoreRes != null)
				{
					return new BaseResponseModel<CommentDto>
					{
						Entity = ObjectMapper.Mapper.Map<CommentDto>(restoreRes),
						Result = DefaultEnums.Result.ok
					};
				}
				else return new BaseResponseModel<CommentDto>(
					new Exception("Failed to restore current comment")
					);
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<CommentDto>>.ErrorFormat(ex);
			}
		}
	}
}

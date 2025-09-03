using ContractsLayer.Base;
using ContractsLayer.Common;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using ContractsLayer.Models;
using DomainLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RepositoryLayer.IRepositories;
using ServicesLayer.IServices;

namespace ServicesLayer.Services
{
	/// <summary>
	/// сервис работы с постами
	/// </summary>
	public class PostsService: IPostsService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly IConfiguration _configuration;
		private readonly IServiceManager _serviceManager;
		private readonly ILogger _logger;

		public PostsService(IRepositoryManager repositoryManager, IConfiguration configuration, IServiceManager serviceManager)
		{
			_repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
			
			var logFactory = LoggerFactory.Create(loggingBuilder => 
			loggingBuilder.SetMinimumLevel(LogLevel.Debug)
				.AddConsole()
			);
			_logger = logFactory.CreateLogger<PostsService>();
		}

		/// <summary>
		/// метод создания публикации
		/// </summary>
		/// <param name="postDto">модель поста</param>
		/// <param name="userName">логин пользователя</param>
		/// <returns></returns>
		public async Task<BaseResponseModel<PostDto>> CreatePost(PostWithFilesCreate postWithFilesDto, HttpContext context)
		{
			try
			{
				// получаем логин пользователя из токена доступа
				var getUserNameResult = await _serviceManager._userService.GetUserNameFromToken(context, _configuration);

				var currentUser = await _serviceManager._userService.GetCurrentUser(getUserNameResult?.Entity);
				if(currentUser?.Error != null)
				{
					return new BaseResponseModel<PostDto>(currentUser.Error);
				}

				// вытаскиваем из присланной модели инф-ю о посте и сохраняем пост в БД
				var currPostDto = postWithFilesDto?.PostDto;
				var postEntry = ObjectMapper.Mapper.Map<Post>(currPostDto);
				var propsToChange = ObjectManager<PostDto>.GetListOfObjectPropertyNames(new PostDto());
				postEntry.userId = currentUser.Entity.id;
				var saveRes = await _repositoryManager._postsRepository.CreatePostEntry(postEntry, propsToChange);

				if(saveRes == null)
				{
					return new BaseResponseModel<PostDto>
					{
						Entity = null,
						Result = DefaultEnums.Result.error,
						Error = new Exception("Failed to save post entry!")
					};
				}
				var mappedEntity = ObjectMapper.Mapper.Map<PostDto>(saveRes);

				// также прикрепляем файлы к текущей публикации
				var attachFilesRes = await _serviceManager._postFilesService
					.AttachFilesToPost(postWithFilesDto?.PostFileIds, saveRes.id);
				if (attachFilesRes?.Result != DefaultEnums.Result.ok)
				{
					return new BaseResponseModel<PostDto>
					{
						Entity = null,
						Result = DefaultEnums.Result.error,
						Error = new Exception($"Failed to save post due to an error that occurred while trying to attach its files: \n {attachFilesRes?.ErrorInfo}" )
					};
				}

				// если все ок, пушим запись в БД эластика
				Task.Run(async () =>
				{
					var elasticSaveRes = await _serviceManager._elasticPostService.CreatePostEntry(mappedEntity);
					if(elasticSaveRes?.Result != DefaultEnums.Result.ok)
					{
						_logger.LogWarning(
							$"An error occurred while trying to upload post to Elastic service. Entity id: {mappedEntity.id}. Error info: \n {elasticSaveRes.ErrorInfo}"
							);
					}
				});



				// если все в итоге правильно выполнилось, сохраняем изменения в БД
				await _repositoryManager._postsRepository.CommitAsync();

				return new BaseResponseModel<PostDto>
				{
					Entity = mappedEntity,
					Result = DefaultEnums.Result.ok
				};
				
				
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<PostDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод получения данных всех пользователей для добавления упоминаний
		/// </summary>
		/// <returns></returns>
		public async Task<List<UserLinkDto>> GetUserLinksForSelect()
		{
			try
			{
				var userData = await _repositoryManager._usersRepository.GetUserIdsWithNames();
				var result = new List<UserLinkDto>();
                foreach (var item in userData)
                {
					result.Add(new UserLinkDto
					{
						userId = item.Key,
						fullName = item.Value
					});
                }
				return result;
            }
			catch (Exception ex)
			{
				return null;
			}
		}

		/// <summary>
		/// метод получения информации о посте по его ID
		/// </summary>
		/// <param name="postId">код поста</param>
		/// <returns></returns>
		public async Task<BaseResponseModel<PostDto>> GetPost(Guid postId)
		{
			try
			{
				var postEntry = await _repositoryManager._postsRepository.GetPostById(postId);
				if(postEntry == null)
				{
					return new BaseResponseModel<PostDto>(new Exception("Failed to load post data"));
				}

				var mappedEntity = ObjectMapper.Mapper.Map<PostDto>(postEntry);
				return new BaseResponseModel<PostDto>
				{
					Entity = mappedEntity,
					Result = DefaultEnums.Result.ok
				};
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<PostDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод получения всех постов, опубликованных текущим (авторизованным в системе) пользователем
		/// </summary>
		/// <param name="userName">логин пользователя</param>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<PostDto>>> GetPostsOfCurrentUser(Guid userId)
		{
			try
			{
				var currentUser = await _serviceManager._userService.GetCurrentUser(userId);
				if (currentUser?.Error != null)
				{
					return new BaseResponseModel<List<PostDto>>(currentUser.Error);
				}

				var postsOfThisUser = await _repositoryManager._postsRepository.GetPostsOfCurrentUser(currentUser.Entity.id);
				var resultList = new List<PostDto>();
				foreach( var post in postsOfThisUser)
				{
					resultList.Add(ObjectMapper.Mapper.Map<PostDto>(post));
				}
				return new BaseResponseModel<List<PostDto>>(resultList);
			}
			catch (Exception ex)
			{
				return new BaseResponseModel<List<PostDto>>(ex);
			}
		}

		/// <summary>
		/// метод редактирования публикации пользователя
		/// </summary>
		/// <param name="postDto">модель публикации</param>
		/// <param name="userName">логин пользователя</param>
		/// <returns></returns>
		public async Task<BaseResponseModel<PostDto>> UpdatePost(PostWithFilesUpdate postUpdateDto, Guid postId, HttpContext context)
		{
			try
			{
				var getUserNameResult =  await _serviceManager._userService.GetUserNameFromToken(context, _configuration);

				// получаем данные о посте из модели
				var postDto = postUpdateDto?.PostDto;

				// проверяем возможность редактирования, затем редактируем пост
				var isAllowedToEdit = await CheckIfUserCanEditPost(postDto.userId, getUserNameResult?.Entity);
				if (isAllowedToEdit?.Error != null)
				{
					return new BaseResponseModel<PostDto>(isAllowedToEdit.Error);
				}

				var propsToUpdate = ObjectManager<PostUpdateDto>.GetListOfObjectPropertyNames(postDto);
				var postEntry = ObjectMapper.Mapper.Map<Post>(postDto);
				postEntry.id = postId;
				var updateRes = await _repositoryManager._postsRepository.UpdatePostEntry(postEntry, propsToUpdate);

				if (updateRes == null)
				{
					return new BaseResponseModel<PostDto>(new Exception("Failed to update current post entry!"));
				}

				var mappedEntity = ObjectMapper.Mapper.Map<PostDto>(updateRes);

				// если все ок с редактированием, пробуем обновить список файлов
				var currExistingFiles = await _repositoryManager._postFilesRepository.GetPostFiles(mappedEntity.id);
				var newFiles = postUpdateDto?.fileIds.Except(currExistingFiles);
				var fileAttachResult = await _serviceManager._postFilesService.AttachFilesToPost(newFiles.ToList(), postId);

				if(fileAttachResult?.Result != DefaultEnums.Result.ok)
				{
					return new BaseResponseModel<PostDto>(
						new Exception($"Failed to update post, could not update file list due to exception: \n {fileAttachResult?.ErrorInfo}")
						);
				}

				// если все прошло успешно, то сохраняем изменения
				await _repositoryManager._postsRepository.CommitAsync();

				Task.Run(async () =>
				{
					var updateElasticRes = await _serviceManager._elasticPostService.UpdatePostEntry(mappedEntity);
					if(updateElasticRes?.Result != DefaultEnums.Result.ok)
					{
						_logger.LogWarning(
							$"An error occurred while trying to update post in Elastic. Entity id: {mappedEntity.id}"
							);
					}
				});

				return new BaseResponseModel<PostDto>
				{
					Entity = mappedEntity,
					Result = DefaultEnums.Result.ok
				};
				
				//else return new BaseResponseModel<PostDto>(new Exception("Failed to update current post entry!"));
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<PostDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод удаления выбранного поста (soft-delete)
		/// </summary>
		/// <param name="postId">код поста</param>
		/// <param name="postUserId">код пользователя - публикатора</param>
		/// <param name="userName">логин пользователя</param>
		/// <returns></returns>
		public async Task<BaseResponseModel<PostDto>> DeletePost(Guid postId, HttpContext context)
		{
			try
			{
				var getUserNameResult = await _serviceManager._userService.GetUserNameFromToken(context, _configuration);
				var curPostEntryOwnerId = await _repositoryManager._postsRepository.GetPostOwnerId(postId);

				var isAllowedToDelete = await CheckIfUserCanEditPost(curPostEntryOwnerId, getUserNameResult?.Entity);
				if(isAllowedToDelete?.Error != null)
				{
					return new BaseResponseModel<PostDto>(isAllowedToDelete.Error);
				}

				// сначала удаляем все файлы, прикрепленные к текущему посту
				var delFilesResult = await _serviceManager._postFilesService.RemoveFilesOfPost(postId);
				if(delFilesResult?.Result != DefaultEnums.Result.ok)
				{
					string ExcMsg = "Failed to remove current post due to error in file deletion: \n" + delFilesResult?.ErrorInfo;
					return new BaseResponseModel<PostDto>(new Exception(ExcMsg));
				}

				// если удаление файлов прошло успешно, помечаем текущий пост удаленным
				var delRes = await _repositoryManager._postsRepository.DeletePostEntry(postId);
				if (delRes != null)
				{
					var mappedEntity = ObjectMapper.Mapper.Map<PostDto>(delRes);
					Task.Run(async () =>
					{
						var elasticDelRes = await _serviceManager._elasticPostService.DeletePostEntry(mappedEntity);
						if(elasticDelRes?.Result != DefaultEnums.Result.ok)
						{
							_logger.LogWarning(
								$"An error occurred while trying to delete post from Elastic. Entity id: {mappedEntity.id}"
								);
						}
					});

					return new BaseResponseModel<PostDto>
					{
						Entity = mappedEntity, 
						Result = DefaultEnums.Result.ok
					};
				}
				else return new BaseResponseModel<PostDto>(new Exception("Post was not deleted due to server error"));
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<PostDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод получения списка постов, удовлетворяющих заданному поисковому термину,
		/// с использованием Elasticsearch
		/// </summary>
		/// <param name="term">поисковый термин</param>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<PostDto>>> SearchPostsElastic(string term, int skip, int take)
		{
			try
			{
				// сначала находим все совпадения в базе эластика
				var searchByTitle = await _serviceManager._elasticPostService.FuzzySearchPosts(term, "title");
				var searchByContent = await _serviceManager._elasticPostService.FuzzySearchPosts(term,"content");
				var searchByPublisher = await _serviceManager._elasticPostService.FuzzySearchPosts(term, "publisherName");

				if(searchByTitle?.Result != DefaultEnums.Result.ok)
				{
					_logger.LogWarning("An exception occurred while searching for posts: " +  searchByTitle?.ErrorInfo);
				}

				if (searchByContent?.Result != DefaultEnums.Result.ok)
				{
					_logger.LogWarning("An exception occurred while searching for posts: " + searchByContent?.ErrorInfo);
				}

				if (searchByPublisher?.Result != DefaultEnums.Result.ok)
				{
					_logger.LogWarning("An exception occurred while searching for posts: " + searchByPublisher?.ErrorInfo);
				}

				// потом извлекаем идентификаторы записей и удаляем среди них повторяющиеся
				var postIds = new List<Guid>();
				postIds.AddRange(searchByTitle.Entity ?? new List<Guid>()
					.Concat(searchByContent.Entity ?? new List<Guid>())
					.Concat(searchByPublisher.Entity ?? new List<Guid>()));

				// берем лишь указанное кол-во записей
				postIds = postIds.Distinct().Skip(skip).Take(take).ToList();

				// и затем возвращаем результат поиска
				var searchResult = await GetPostsList(postIds);
				return searchResult;
				
			}
			catch (Exception ex)
			{
				return new BaseResponseModel<List<PostDto>>(ex);
			}
		}


		/// <summary>
		/// метод получения списка постов по их ID
		/// </summary>
		/// <param name="postIds"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<PostDto>>> GetPostsList(List<Guid> postIds)
		{
			try
			{
				var searcResult = await _repositoryManager._postsRepository.GetAllPosts(p => postIds.Contains(p.id));
				var mappedEntity = ObjectMapper.Mapper.Map<List<PostDto>>(searcResult);
				return new BaseResponseModel<List<PostDto>>(mappedEntity);
			}
			catch (Exception ex)
			{
				return new BaseResponseModel<List<PostDto>>(ex);
			}
		}

		/// <summary>
		/// метод проверки наличия прав
		/// для текущего пользователя на редактирование/ удаление поста
		/// </summary>
		/// <param name="postOwnerId">код владельца поста</param>
		/// <param name="userName">логин пользователя, вошедшего в систему</param>
		/// <returns></returns>
		private async Task<BaseModel> CheckIfUserCanEditPost(Guid postOwnerId, string userName)
		{
			try
			{
				var currentUser = await _serviceManager._userService.GetCurrentUser(userName);
				if(currentUser?.Error != null)
					return new BaseModel(currentUser.Error);

				if (currentUser?.Entity?.id != postOwnerId)
					return new BaseModel(new Exception("You're not allowed to work with this post!"));

				return new BaseModel();
			}
			catch (Exception ex)
			{
				return new BaseModel(ex);
			}
		}

	}
}

using ContractsLayer.Base;
using ContractsLayer.Dtos;
using RepositoryLayer.IRepositories;
using ServicesLayer.IServices;

namespace ServicesLayer.Services
{
	/// <summary>
	/// сервис работы с хештегами
	/// </summary>
	public class HashTagService: IHashTagService
	{
		private readonly IRepositoryManager _repositoryManager;

		public HashTagService(IRepositoryManager repositoryManager)
		{
			_repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
		}

		/// <summary>
		/// метод получения списка всех хештегов
		/// </summary>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<HashTagDto>>> GetAllHashTags(int skip, int take)
		{
			try
			{
				var hashTags = await _repositoryManager._hashTagRepository.GetAllTags(skip, take);
				if(hashTags?.Count == 0)
				{
					return new BaseResponseModel<List<HashTagDto>>(new Exception("No hashtags were created yet"));
				}
				else
				{
					var mappedEntity = ObjectMapper.Mapper.Map<List<HashTagDto>>(hashTags);
					return new BaseResponseModel<List<HashTagDto>>(mappedEntity);
				}
			}
			catch(Exception ex)
			{
				return new BaseResponseModel<List<HashTagDto>>(ex);
			}
		}

		/// <summary>
		/// метод создания записи о хештеге
		/// </summary>
		/// <param name="hashTag"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<HashTagDto>> CreateHashTag(string hashTag)
		{
			try
			{
				var createdEntity = await _repositoryManager._hashTagRepository.AddHasTagEntry(hashTag);
				if(createdEntity != null)
				{
					var mappedEntity = ObjectMapper.Mapper.Map<HashTagDto>(createdEntity);
					return new BaseResponseModel<HashTagDto>(mappedEntity);
				}
				else
				{
					return new BaseResponseModel<HashTagDto>(new Exception("An error occurred while trying to save hash tag entry"));
				}
			}
			catch(Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<HashTagDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод удаления хештега
		/// </summary>
		/// <param name="hasTagId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<HashTagDto>> RemoveHashTag(Guid hasTagId)
		{
			try
			{
				var removeRes = await _repositoryManager._hashTagRepository.DeleteHashTag(hasTagId);
				if(removeRes != null)
				{
					var mappedEntity = ObjectMapper.Mapper.Map<HashTagDto>(removeRes);
					return new BaseResponseModel<HashTagDto>( mappedEntity);
				}
				else
				{
					return new BaseResponseModel<HashTagDto>(new Exception("An error occurred while deleting hash tag"));
				}

			}
			catch(Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<HashTagDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод поиска хештегов по содержимому (получение набора значений с пагинацией)
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<HashTagDto>>> GetHashTagsByContent(string content, int skip, int take)
		{
			try
			{
				var searchResults = await _repositoryManager._hashTagRepository.GetHashTags(content, skip, take);
				if(searchResults?.Count > 0)
				{
					var mappedEntity = ObjectMapper.Mapper.Map<List<HashTagDto>>(searchResults);
					return new BaseResponseModel<List<HashTagDto>>(mappedEntity);
				}
				else
				{
					return new BaseResponseModel<List<HashTagDto>>(new Exception("No tags with similar content were found"));
				}
			}
			catch(Exception ex)
			{
				return new BaseResponseModel<List<HashTagDto>>(ex);
			}
		}
	}
}

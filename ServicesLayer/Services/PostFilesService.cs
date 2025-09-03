using ContractsLayer.Base;
using ContractsLayer.Common;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using RepositoryLayer.IRepositories;
using ServicesLayer.IServices;


namespace ServicesLayer.Services
{
	/// <summary>
	/// сервис связи файлов с постами
	/// </summary>
	public class PostFilesService: IPostFilesService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly IServiceManager _serviceManager;

		public PostFilesService(IRepositoryManager repositoryManager, IServiceManager serviceManager)
		{
			_repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
		}

		/// <summary>
		/// метод прикрепления списка файлов к посту
		/// </summary>
		/// <param name="fileIds"></param>
		/// <param name="postId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<PostFileDto>>> AttachFilesToPost(List<Guid> fileIds, Guid postId)
		{
			try
			{
				// проверяем, что файлы с данными ID действительно существуют в БД
				var filesListCorrect = await _repositoryManager._fileDataRepository.CheckIfFilesExist(fileIds);
				if (!filesListCorrect)
				{
					return new BaseResponseModel<List<PostFileDto>>
					(
						new Exception("Received files list is incorrect!")
					);
				}

				// затем сохраняем связь данных файлов с выбранным постом в БД
				var addResult = await _repositoryManager._postFilesRepository.AddPostFiles(postId, fileIds);
				if(addResult?.Count > 0)
				{
					return new BaseResponseModel<List<PostFileDto>>
					{
						Entity = ObjectMapper.Mapper.Map<List<PostFileDto>>(addResult),
						Result = DefaultEnums.Result.ok,
					};
				}
				else
				{
					return new BaseResponseModel<List<PostFileDto>>(new Exception("An error occurred while attempting to attach files"));
				}
			}
			catch(Exception ex)
			{
				return new BaseResponseModel<List<PostFileDto>>(ex);
			}
		}

		/// <summary>
		/// метод открепления и удаления файла
		/// </summary>
		/// <param name="postId"></param>
		/// <param name="fileId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<PostFileDto>> DetachFileFromPost(Guid postId, Guid fileId)
		{
			try
			{
				// открепляем файл от родительского поста
				var delPostFileResult = await _repositoryManager._postFilesRepository.DeletePostFile(postId, fileId);
				if(delPostFileResult == null)
				{
					return new BaseResponseModel<PostFileDto>(new Exception("Failed to remove file from current post"));
				}

				// если открепление прошло успешно, тогда удаляем файл
				var currentFile = await _repositoryManager._fileDataRepository.GetFile(fileId);
				var delFromMinioRes = await _serviceManager._filesService.RemoveFileFromMinio(currentFile?.fileName);
				if( delFromMinioRes?.Result != DefaultEnums.Result.ok)
				{
					return new BaseResponseModel<PostFileDto>(delFromMinioRes?.Error);
				}

				var delFromDatabaseResult = await _serviceManager._filesService.DeleteFileEntry(currentFile.id);
				if(delFromDatabaseResult.Result != DefaultEnums.Result.ok)
				{
					return new BaseResponseModel<PostFileDto>(delFromDatabaseResult?.Error);
				}

				return new BaseResponseModel<PostFileDto>
				{
					Entity = ObjectMapper.Mapper.Map<PostFileDto>(delPostFileResult),
					Result = DefaultEnums.Result.ok
				};
			}
			catch(Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<PostFileDto>>.ErrorFormat(ex);
			}
		}


		/// <summary>
		/// метод получения списка ID файлов текущего поста
		/// </summary>
		/// <param name="postId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<Guid>>> GetFilesAttachedToPost(Guid postId)
		{
			try
			{
				var result = await _repositoryManager._postFilesRepository.GetPostFiles(postId);
				return new BaseResponseModel<List<Guid>>(result);
			}
			catch(Exception ex)
			{
				return new BaseResponseModel<List<Guid>>(ex);
			}
		}

		/// <summary>
		/// метод удаления файлов, прикрепленных к выбранному посту
		/// </summary>
		/// <param name="postId">код выбранного поста </param>
		/// <returns></returns>
		public async Task<BaseModel> RemoveFilesOfPost(Guid postId)
		{
			try
			{
				// проверяем, прикреплены ли файлы к текущему посту
				var postFiles = await _repositoryManager._postFilesRepository.GetPostFiles(postId);
				if(postFiles?.Count == 0)
				{
					return new BaseModel();
				}
				// сначала удаляем все связи между файлами и выбранным постом
				var postFilesDelResult = await _repositoryManager._postFilesRepository
					.DeletePostFiles(postId, postFiles, withCommit: true);

				if(postFilesDelResult?.Count != postFiles.Count) {
					throw new Exception("Files of current post haven't been properly detached!");
				}

				// если все ок, то удаляем сами файлы
				var filesToDelete = await _repositoryManager._fileDataRepository
					.GetAsync(f => postFiles.Contains(f.id));
				var fileIds = filesToDelete.Select(f => f.id).ToList();
				var fileNames = filesToDelete.Select(f => f.fileName).ToList();

				// сначала из MinIO
				var minIODelRes = await _serviceManager._filesService.RemoveFilesFromMinio(fileNames);
				if(minIODelRes?.Result != DefaultEnums.Result.ok)
				{
					return new BaseModel(minIODelRes.Error);
				}

				// затем из БД
				var databaseDelRes = await _serviceManager._filesService.DeleteFileEntries(fileIds);
				if(databaseDelRes?.Result != DefaultEnums.Result.ok)
				{
					return new BaseModel(databaseDelRes?.Error);
				}

				return new BaseModel();

			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseModel>.ErrorFormat(ex);
			}
		}
	}
}

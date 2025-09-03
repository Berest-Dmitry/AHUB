using ContractsLayer.Base;
using ContractsLayer.Common;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using DomainLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Minio;
using RepositoryLayer.IRepositories;
using ServicesLayer.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Services
{
	/// <summary>
	/// сервис работы с файлами
	/// </summary>
	public class FilesService: IFilesService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly IConfiguration _configuration;
		private readonly MinioClient _minioClient;

		public FilesService(IRepositoryManager repositoryManager, IConfiguration configuration)
		{
			_repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_minioClient = new MinioClient()
				.WithEndpoint(configuration["MinioSettings:endpoint"])
				.WithCredentials(configuration["MinioSettings:accessKey"], configuration["MinioSecret"])
				.Build();
		}

		/// <summary>
		/// метод отправки файла в облако
		/// </summary>
		/// <param name="fileData"></param>
		/// <returns></returns>
		public async Task<BaseModel> UploadFileToMinIO(FileDataDto fileData, Stream ms)
		{
			try
			{
				var beArgs = new BucketExistsArgs().WithBucket(_configuration["MinioSettings:bucketName"]);
				bool found = await _minioClient.BucketExistsAsync(beArgs);
				if (!found)
				{
					var cbArgs = new MakeBucketArgs().WithBucket(_configuration["MinioSettings:bucketName"]);
					await _minioClient.MakeBucketAsync(cbArgs).ConfigureAwait(false);
				}

				var putObjectArgs = new PutObjectArgs()
					.WithBucket(_configuration["MinioSettings:bucketName"])
					.WithObject(fileData.fileName)
					.WithStreamData(ms)
					.WithObjectSize(ms.Length)
					.WithContentType(fileData.mediaType);
					

				var putResult = await _minioClient.PutObjectAsync(putObjectArgs);
				ms.Close();
				ms.Dispose();
				if (putResult != null && !string.IsNullOrEmpty(putResult.ObjectName))
				{
					return new BaseModel();
				}
				else return new BaseModel(new Exception("Failed to upload chosen file!"));
			}
			catch(Exception ex)
			{
				return new BaseModel(ex);
			}
		}

		/// <summary>
		/// метод загрузки файла на сервер пользователем
		/// </summary>
		/// <returns></returns>
		public async Task<BaseResponseModel<FileDataDto>> UploadFile(IFormFile formFile)
		{
			try
			{
				// check if uploaded file is larger than 24 MBytes, if so, throw an error
				if(formFile.Length > 25165824)
				{
					return new BaseResponseModel<FileDataDto>(new Exception("File is too large"));
				}

				if (!DefaultEnums.AllowedFileTypes.ContainsValue(formFile.ContentType))
				{
					return new BaseResponseModel<FileDataDto>(new Exception("It's not allowed to upload file of this type"));
				}

				var fileData = new FileDataDto
				{
					fileName = formFile.FileName,
					mediaType = MIMEHelper.GetMIMEType(formFile.FileName),
					bucketName = _configuration["MinioSettings:bucketName"]
				};

				var stream = formFile.OpenReadStream();

				var uploadRes = await UploadFileToMinIO(fileData, stream);
				if (uploadRes?.Result == DefaultEnums.Result.error)
				{
					return new BaseResponseModel<FileDataDto>(new Exception("Failed to upload file to cloud storage"));
				}

				var fileSaveRes = await SaveFileEntry(fileData);
				return fileSaveRes;
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<FileDataDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод сохранения сущности файла в БД
		/// </summary>
		/// <param name="fileData"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<FileDataDto>> SaveFileEntry(FileDataDto fileData)
		{
			try
			{
				var file = ObjectMapper.Mapper.Map<FileData>(fileData);
				var fileProps = ObjectManager<FileDataDto>.GetListOfObjectPropertyNames(fileData);
				var saveResult = await _repositoryManager._fileDataRepository.AddFileEntry(file, fileProps);
				if (saveResult != null)
				{
					return new BaseResponseModel<FileDataDto>
					{
						Entity = ObjectMapper.Mapper.Map<FileDataDto>(saveResult),
						Result = DefaultEnums.Result.ok
					};
				}
				else
				{
					return new BaseResponseModel<FileDataDto>
					{
						Entity = null,
						Result = DefaultEnums.Result.error,
						Error = new Exception("An error occurred while trying to save file data entry")
					};
				}
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<FileDataDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод скачивания файла на клиент
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public async Task<FileDownloadDto> DownloadFile(string fileName)
		{
			try
			{
				var ms = new MemoryStream();
				var statObjectArgs = new StatObjectArgs()
					.WithBucket(_configuration["MinioSettings:bucketName"])
					.WithObject(fileName);

				await _minioClient.StatObjectAsync(statObjectArgs);

				var getObjectArgs = new GetObjectArgs()
					.WithBucket(_configuration["MinioSettings:bucketName"])
					.WithObject(fileName)
					.WithCallbackStream((stream) =>
					{
						stream.CopyTo(ms);
						ms.Seek(0, SeekOrigin.Begin);
						stream.Close();
						stream.Dispose();
					});

				var result = await _minioClient.GetObjectAsync(getObjectArgs);
				if(result != null && result.ObjectName == fileName)
				{
					return new FileDownloadDto
					{
						stream = ms,
						contentType = MIMEHelper.GetMIMEType(result.ObjectName)
					};
				}
				else
				{
					return null;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// метод удаления объекта из хранилища minio
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public async Task<BaseModel> RemoveFileFromMinio(string fileName)
		{
			try
			{
				var removeObjectArgs = new RemoveObjectArgs()
					.WithBucket(_configuration["MinioSettings:bucketName"])
					.WithObject(fileName);

				await _minioClient.RemoveObjectAsync(removeObjectArgs);
				return new BaseModel();
			}
			catch (Exception ex)
			{
				return new BaseModel(ex);
			}
		}

		/// <summary>
		/// метод удаления записи о файле из БД
		/// </summary>
		/// <param name="fileId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<FileDataDto>> DeleteFileEntry(Guid fileId)
		{
			try
			{
				var delResult = await _repositoryManager._fileDataRepository.DeleteFileEntry(fileId);
				if (delResult != null)
				{
					return new BaseResponseModel<FileDataDto>
					{
						Entity = ObjectMapper.Mapper.Map<FileDataDto>(delResult),
						Result = DefaultEnums.Result.ok
					};
				}
				else
				{
					return new BaseResponseModel<FileDataDto>(new Exception("Failed to remove file entry"));
				}
			}
			catch (Exception ex) { 
				return BaseModelUtilities<BaseResponseModel<FileDataDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод загрузки файла на сторону клиента
		/// </summary>
		/// <param name="fileId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<FileDownloadDto>> GetFile(Guid fileId)
		{
			try
			{
				var fileInfo = await _repositoryManager._fileDataRepository.GetFile(fileId);
				if (fileInfo == null)
				{
					return new BaseResponseModel<FileDownloadDto>(new Exception("File wasn't found!"));
				}

				var fileResult = await DownloadFile(fileInfo?.fileName);
				if (fileResult == null)
				{
					return new BaseResponseModel<FileDownloadDto>(new Exception("Failed to load file from storage!"));
				}

				fileResult.fileName = fileInfo?.fileName;
				return new BaseResponseModel<FileDownloadDto>(fileResult);
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<FileDownloadDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод удаления файла
		/// </summary>
		/// <param name="fileId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<BaseModel>> DeleteFile(Guid fileId)
		{
			try
			{
				var currentFile = await _repositoryManager._fileDataRepository.GetFile(fileId);
				var delFromMinioRes = await RemoveFileFromMinio(currentFile?.fileName);
				if (delFromMinioRes?.Result != DefaultEnums.Result.ok)
				{
					return new BaseResponseModel<BaseModel>(delFromMinioRes?.Error);
				}

				var delFromDBResult = await DeleteFileEntry(currentFile.id);
				if(delFromDBResult?.Result != DefaultEnums.Result.ok)
				{
					return new BaseResponseModel<BaseModel>(delFromDBResult?.Error);
				}

				return new BaseResponseModel<BaseModel>();
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<BaseModel>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод множественного удаления файлов из хранилища
		/// </summary>
		/// <param name="files"></param>
		/// <returns></returns>
		public async Task<BaseModel> RemoveFilesFromMinio(List<string> files)
		{
			try
			{
				string exceptionMessage = "", failedToRemoveObjectsMessage = "";
				var removeObjectArgs = new RemoveObjectsArgs()
					.WithBucket(_configuration["MinioSettings:bucketName"])
					.WithObjects(files);

				var observable = await _minioClient.RemoveObjectsAsync(removeObjectArgs);
				var subscription = observable.Subscribe(delError =>
				{
					failedToRemoveObjectsMessage += $"Failed to remove object: {delError.Key} \n";
				},
				ex =>
				{
					exceptionMessage += $"An error occured while removing file: {ex.Message} \n";
				},
				() => { }
				);

				if(!string.IsNullOrEmpty(exceptionMessage) || !string.IsNullOrEmpty(failedToRemoveObjectsMessage)) { 
					return new BaseResponseModel<BaseModel>(new Exception(failedToRemoveObjectsMessage + exceptionMessage));
				}
				return new BaseResponseModel<BaseModel>();
			}
			catch(Exception ex) {
				return new BaseModel(ex);
			}
		}

		/// <summary>
		/// метод удаления списка файлов из БД
		/// </summary>
		/// <param name="fileIds"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<Guid>>> DeleteFileEntries(List<Guid> fileIds)
		{
			try
			{
				var delResult = await _repositoryManager._fileDataRepository.DeleteFiles(fileIds);
				if( delResult?.Count > 0)
				{
					return new BaseResponseModel<List<Guid>>(delResult);
				}
				else
				{
					return new BaseResponseModel<List<Guid>>(new Exception("An error occurred while removing files from database!"));
				}
			}
			catch (Exception ex)
			{
				return new BaseResponseModel<List<Guid>>(ex);
			}
		}

		/// <summary>
		/// метод удаления неиспользоватнных файлов по расписанию
		/// </summary>
		/// <returns></returns>
		public async Task<BaseModel> CheckAndRemoveUnusedFiles()
		{
			try
			{
				var unusedFiles = await _repositoryManager._fileDataRepository.GetAsync(f => f.entityOwnerId == null);
				if(unusedFiles?.Count > 0)
				{
					var fileIds = unusedFiles.Select(f => f.id).ToList();
					var fileNames = unusedFiles.Select(f => f.fileName).ToList();

					var delFromMinioResult = await RemoveFilesFromMinio(fileNames);
					if(delFromMinioResult?.Result == DefaultEnums.Result.error)
					{
						return new BaseModel(delFromMinioResult?.Error);
					}

					var delFromDataBaseResult = await DeleteFileEntries(fileIds);
					if(delFromDataBaseResult?.Result == DefaultEnums.Result.error)
					{
						return new BaseModel(delFromDataBaseResult?.Error);
					}
					return new BaseModel();
				}
				else return new BaseModel();
			}
			catch (Exception ex)
			{
				return new BaseModel(ex);
			}
		}
	}
}

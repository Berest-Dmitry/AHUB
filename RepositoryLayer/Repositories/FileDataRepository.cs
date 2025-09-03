using DomainLayer.Data;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.IRepositories;
using RepositoryLayer.Repositories.Base;

namespace RepositoryLayer.Repositories
{
	/// <summary>
	/// репозиторий для работы с файлами
	/// </summary>
	public class FileDataRepository: Repository<FileData>, IFileDataRepository
	{
		public FileDataRepository(AHUBContext dbContext) : base(dbContext) { }

		/// <summary>
		/// метод получения списка файлов по их ID
		/// </summary>
		/// <param name="fileIds"></param>
		/// <returns></returns>
		public async Task<List<FileData>> GetListOfFiles(List<Guid> fileIds)
		{
			return await _dbContext.Files.Where(f => fileIds.Contains(f.id)).ToListAsync();
		}

		/// <summary>
		/// метод проверки правильности передачи списка кодов файлов для сохранения
		/// </summary>
		/// <param name="fileIds"></param>
		/// <returns></returns>
		public async Task<bool> CheckIfFilesExist(List<Guid> fileIds)
		{
			return await _dbContext.Files
				.Where(f => fileIds.Contains(f.id))
				.CountAsync() == fileIds.Count;
		}

		/// <summary>
		/// метод получения файла по его ID
		/// </summary>
		/// <param name="fileId"></param>
		/// <returns></returns>
		public async Task<FileData> GetFile(Guid fileId)
		{
			return await GetByIdAsync(fileId);
		}

		/// <summary>
		/// метод создания записи о файле
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public async Task<FileData> AddFileEntry(FileData file, List<string> propsToChange)
		{
			var fileEntry = new FileData();
			FillEntityData(file, ref fileEntry, propsToChange);
			await AddAsync(fileEntry);
			return fileEntry;
		}

		/// <summary>
		/// метод удаления записи о файле
		/// </summary>
		/// <param name="fileId"></param>
		/// <returns></returns>
		public async Task<FileData> DeleteFileEntry(Guid fileId)
		{
			var existingFile = await GetByIdAsync(fileId);
			_dbContext.Entry(existingFile).State = EntityState.Deleted;
			await _dbContext.SaveChangesAsync();
			return existingFile;
		}

		/// <summary>
		/// иетод удаления списка файлов из БД
		/// </summary>
		/// <param name="fileIds"></param>
		/// <returns></returns>
		public async Task<List<Guid>> DeleteFiles(List<Guid> fileIds)
		{
			var existingFiles = await GetAsync(f => fileIds.Contains(f.id));
			existingFiles.ForEach((file) =>
			{
				_dbContext.Entry(file).State = EntityState.Deleted;
			});
			await _dbContext.SaveChangesAsync();
			return existingFiles.Select(f => f.id).ToList();
		}
	}
}

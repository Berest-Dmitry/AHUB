using DomainLayer.Data;
using DomainLayer.Models.Base;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.IRepositories.Base;
using System.Collections;
using System.Linq.Expressions;


namespace RepositoryLayer.Repositories.Base
{
	/// <summary>
	/// базовый репозиторий приложения
	/// </summary>
	/// <typeparam name="T">Тип сущности из БД</typeparam>
	public class Repository<T>: IRepository<T> where T: EntityBase
	{

		protected readonly AHUBContext _dbContext;

		public Repository(AHUBContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public async Task<List<T>> GetAllAsync()
		{
			return await _dbContext.Set<T>().ToListAsync();
		}

		public async Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbContext.Set<T>().Where(predicate).ToListAsync();
		}

		public async Task<T> GetByIdAsync(Guid id)
		{
			return await _dbContext.Set<T>().Where(e => e.id == id).FirstOrDefaultAsync();
		}

		public async Task<T> AddAsync(T entity)
		{
			try
			{
				_dbContext.Set<T>().Add(entity);
				await _dbContext.SaveChangesAsync();
				return entity;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<T> UpdateAsync(T entity)
		{
			try
			{
				_dbContext.Entry(entity).State = EntityState.Modified;
				await _dbContext.SaveChangesAsync();
				return entity;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<T> DeleteAsync(T entity)
		{
			try
			{
				_dbContext.Set<T>().Remove(entity);
				await _dbContext.SaveChangesAsync();
				return entity;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task CommitAsync()
		{
			await _dbContext.SaveChangesAsync();
		}

		public Dictionary<string, object> GetPropertiesWithValues(T entity)
		{
			Dictionary<string, object> result = new Dictionary<string, object>();
			var props = typeof(T).GetProperties();
			foreach (var prop in props)
			{
				var value = prop.GetValue(entity);
				result.Add(prop.Name, value);
			}
			return result;
		}

		/// <summary>
		/// метод заполнения сущности полями из модели представления
		/// </summary>
		/// <param name="entry">модель представления</param>
		/// <param name="existingEntry">сущность из БД</param>
		/// <param name="propsToChange">список полей модели, которые нужно (возможно) поменять в сущности</param>
		public void  FillEntityData(T entry, ref T existingEntry, List<string> propsToChange)
		{
			var entityProps = GetPropertiesWithValues(entry);
			var entityToUpdate = typeof(T).GetProperties();

			// список полей, которые менять запрещено
			var propsToMaintain = new List<string>() { "id", "dateTimeAdded" };
			foreach (var prop in entityProps.Where(p => !propsToMaintain.Contains(p.Key) 
				&& propsToChange.Contains(p.Key)))
			{
				var currentProp = entityToUpdate.Where(pr => pr.Name == prop.Key).FirstOrDefault();

				var thisValue = currentProp.GetValue(existingEntry);
				if (Comparer.DefaultInvariant.Compare(thisValue, prop.Value) != 0)
				{
					currentProp.SetValue(existingEntry, prop.Value);
				}
			}

		}

	}
}

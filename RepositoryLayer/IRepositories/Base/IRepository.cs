using DomainLayer.Models.Base;
using System.Linq.Expressions;

namespace RepositoryLayer.IRepositories.Base
{
	/// <summary>
	/// интерфейс базового репозитория
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IRepository<T> where T : EntityBase
	{
		Task<List<T>> GetAllAsync();

		Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate);

		Task<T> GetByIdAsync(Guid id);

		Task<T> AddAsync(T entity);

		Task<T> UpdateAsync(T entity);

		Task<T> DeleteAsync(T entity);

		Task CommitAsync();

		Dictionary<string, object> GetPropertiesWithValues(T entity);

		void FillEntityData(T entry, ref T existingEntry, List<string> propsToChange);
	}
}

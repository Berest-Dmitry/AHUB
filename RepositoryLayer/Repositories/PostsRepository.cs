using DomainLayer.Data;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.IRepositories;
using RepositoryLayer.Repositories.Base;
using System.Linq.Expressions;


namespace RepositoryLayer.Repositories
{
	/// <summary>
	/// репозиторий сущности "Post"
	/// </summary>
	public class PostsRepository: Repository<Post>, IPostsRepository
	{
		public PostsRepository(AHUBContext dbContext) : base(dbContext) { }

		/// <summary>
		/// метод получения записи о посте (публикации) по ее ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<Post> GetPostById(Guid id)
		{
			return await GetByIdAsync(id);
		}

		/// <summary>
		/// метод получения всех публикаций конкретного пользователя
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<List<Post>> GetPostsOfCurrentUser(Guid userId)
		{
			return await _dbContext.Posts.Where(p => p.userId == userId && p.deletedAt == null).ToListAsync();
		}

		/// <summary>
		/// метод получения заданного кол-ва постов (публикаций), удовлетворяющих условию
		/// </summary>
		/// <param name="predicate">условие</param>
		/// <param name="skip">сколько пропустить</param>
		/// <param name="take">сколько взять</param>
		/// <returns></returns>
		public async Task<List<Post>> GetAllPosts(Expression<Func<Post, bool>> predicate, int skip, int take)
		{
			return await _dbContext.Posts.Where(predicate).Skip(skip).Take(take).ToListAsync();	
		}

		/// <summary>
		/// метод получения всех постов (публикаций), удовлетворяющих условию
		/// </summary>
		/// <param name="predicate">условие</param>
		/// <returns></returns>
		public async Task<List<Post>> GetAllPosts(Expression<Func<Post, bool>> predicate)
		{
			return await _dbContext.Posts.Where(predicate).ToListAsync();
		}

		/// <summary>
		/// метод создания сущности публикации
		/// </summary>
		/// <param name="post"></param>
		/// <returns></returns>
		public async Task<Post> CreatePostEntry(Post post, List<string> propsToChange, bool withCommit = false)
		{
			var postEntry = new Post();
			FillEntityData(post, ref  postEntry, propsToChange);
			if(withCommit)
			{
				await AddAsync(postEntry);
			}
			else
			{
				_dbContext.Entry(postEntry).State = EntityState.Added;
			}

			return postEntry;
		}

		/// <summary>
		/// метод редактирования сущности публикации
		/// </summary>
		/// <param name="post"></param>
		/// <returns></returns>
		public async Task<Post> UpdatePostEntry(Post post, List<string> propsToChange, bool withCommit = false)
		{
			var existingPost = await GetByIdAsync(post.id);
			FillEntityData(post, ref existingPost, propsToChange);
			_dbContext.Entry(existingPost).State = EntityState.Modified;

			if (withCommit)
			{
				await _dbContext.SaveChangesAsync();
			}
			return existingPost;
		}

		/// <summary>
		/// метод обозначения выбранной записи о публикации удаленной
		/// </summary>
		/// <param name="postId"></param>
		/// <returns></returns>
		public async Task<Post> DeletePostEntry(Guid postId)
		{
			var existingPost = await GetByIdAsync(postId);
			existingPost.deletedAt = DateTime.UtcNow;
			_dbContext.Entry(existingPost).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
			return existingPost;
		}

		/// <summary>
		/// метод получения ID пользователя - создателя поста
		/// </summary>
		/// <param name="postId">код поста</param>
		/// <returns></returns>
		public async Task<Guid> GetPostOwnerId(Guid postId)
		{
			return await _dbContext.Posts.Where(p => p.id == postId)
				.Select(p => p.userId).FirstOrDefaultAsync();
		}
	}
}

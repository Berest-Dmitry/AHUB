using DomainLayer.Models;
using RepositoryLayer.IRepositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.IRepositories
{
	/// <summary>
	/// интерфейс репозитория сущности "Post"
	/// </summary>
	public interface IPostsRepository: IRepository<Post>
	{

		Task<Post> GetPostById(Guid id);

		Task<List<Post>> GetPostsOfCurrentUser(Guid userId);

		Task<List<Post>> GetAllPosts(Expression<Func<Post, bool>> predicate, int skip, int take);

		Task<List<Post>> GetAllPosts(Expression<Func<Post, bool>> predicate);

		Task<Post> CreatePostEntry(Post post, List<string> propsToChange, bool withCommit = false);

		Task<Post> UpdatePostEntry(Post post, List<string> propsToChange, bool withCommit = false);

		Task<Post> DeletePostEntry(Guid postId);

		Task<Guid> GetPostOwnerId(Guid postId);
	}
}

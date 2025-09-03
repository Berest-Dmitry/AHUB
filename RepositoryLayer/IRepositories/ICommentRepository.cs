using DomainLayer.Models;
using RepositoryLayer.IRepositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.IRepositories
{
	/// <summary>
	/// интерфейс репозитория комментариев
	/// </summary>
	public interface ICommentRepository: IRepository<Comment>
	{
		Task<List<Comment>> GetCommentsAttachedToPost(Guid postId, int skip, int take);

		Task<List<Comment>> GetListOfChildComments(Guid parentId, int skip, int take);

		Task<List<KeyValuePair<Guid, int>>> GetTotalCountOfChildComments(List<Guid> parents);

		Task<Comment> CreateCommentEntry(Comment comment, List<string> propsToChange);

		Task<Comment> UpdateCommentEntry(Comment comment,string newContent);

		Task<Comment> DeleteOrRestoreCommentEntry(Guid commentId, bool toBeRestored = false);

	}
}

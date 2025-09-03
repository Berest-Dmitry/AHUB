using DomainLayer.Data;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.IRepositories;
using RepositoryLayer.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Repositories
{
	/// <summary>
	/// репозиторий сущности комментариев
	/// </summary>
	public class CommentRepository: Repository<Comment>, ICommentRepository
	{

		public CommentRepository(AHUBContext dbContext) : base(dbContext) { }

		/// <summary>
		/// метод получения списка комментариев к посту, отсортированных по дате добавления по убыванию 
		/// только комментарии верхнего уровня
		/// </summary>
		/// <param name="postId">код поста</param>
		/// <param name="skip">пропустить</param>
		/// <param name="take">взять</param>
		/// <returns></returns>
		public async Task<List<Comment>> GetCommentsAttachedToPost(Guid postId, int skip, int take)
		{
			return await _dbContext.Comments.Where(c => c.postId == postId && c.parentId == null)
				.OrderByDescending(c => c.dateTimeAdded).Skip(skip).Take(take)
				.ToListAsync();
		}

		/// <summary>
		/// метод получения набора родительсмких комментариев с кол-вом дочерних для каждого родительского
		/// нужно для отображения кол-ва дочерних комментариев в нераскрытой ветке
		/// </summary>
		/// <param name="parents">список ID родительских комментариев</param>
		/// <returns></returns>
		public async Task<List<KeyValuePair<Guid, int>>> GetTotalCountOfChildComments(List<Guid> parents)
		{
			var children = await _dbContext.Comments
				.Where(x => parents.Contains(x.parentId ?? Guid.Empty))
				.Select(x => new {x.id, x.parentId})
				.ToListAsync();

			var result = new List<KeyValuePair<Guid, int>>();
			foreach(var group in children.GroupBy(x => x.parentId))
			{
				result.Add(new KeyValuePair<Guid, int>(group.Key ?? Guid.Empty, group.Count()));
			}
			return result;
		}

		/// <summary>
		/// метод создания записи о комментарии
		/// </summary>
		/// <param name="comment"></param>
		/// <param name="propsToChange"></param>
		/// <returns></returns>
		public async Task<Comment> CreateCommentEntry(Comment comment, List<string> propsToChange)
		{
			var commentEntry = new Comment();
			FillEntityData(comment, ref commentEntry, propsToChange);
			await AddAsync(commentEntry);
			return commentEntry;
		}

		/// <summary>
		/// метод редактирования комментария
		/// </summary>
		/// <param name="commentId"></param>
		/// <param name="newContent"></param>
		/// <returns></returns>
		public async Task<Comment> UpdateCommentEntry(Comment comment, string newContent)
		{
			comment.content = newContent;
			comment.dateTimeEdited = DateTime.UtcNow;
			_dbContext.Entry(comment).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
			return comment;
		}

		/// <summary>
		/// метод удаления/ восстановления комментария
		/// </summary>
		/// <param name="commentId"></param>
		/// <returns></returns>
		public async Task<Comment> DeleteOrRestoreCommentEntry(Guid commentId, bool toBeRestored = false)
		{
			var existingEntry = await GetByIdAsync(commentId);
			existingEntry.deletedAt = toBeRestored ? null : DateTime.UtcNow;
			_dbContext.Entry(existingEntry).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
			return existingEntry;
		}

		/// <summary>
		/// метод получения списка комментариев-ответов на текущий комментарий
		/// </summary>
		/// <param name="parentId">код родительского (текущего) комментария</param>
		/// <param name="skip">пропустить</param>
		/// <param name="take">взять</param>
		/// <returns></returns>
		public async Task<List<Comment>> GetListOfChildComments(Guid parentId, int skip, int take)
		{
			return await _dbContext.Comments.Where(c => c.parentId == parentId)
				.OrderByDescending(c => c.dateTimeAdded).Skip(skip).Take(take)
				.ToListAsync();
		}
	}
}

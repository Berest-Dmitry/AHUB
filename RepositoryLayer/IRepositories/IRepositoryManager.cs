using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.IRepositories
{
	/// <summary>
	/// интерфейс менеджера репозиториев проекта
	/// </summary>
	public interface IRepositoryManager
	{
		IUsersRepository _usersRepository { get; }
		IPasswordChangesRepository _passwordChangesRepository { get; }

		IFileDataRepository _fileDataRepository { get; }

		IPostsRepository _postsRepository { get; }

		IPostFilesRepository _postFilesRepository { get; }

		IHashTagRepository _hashTagRepository { get; }

		ICommentRepository _commentRepository { get; }

		IAppealRepository _appealRepository { get; }

		IRoleRepository _roleRepository { get; }

		IUserRolesRepository _userRolesRepository { get; }
	}
}

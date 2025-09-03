using DomainLayer.Data;
using RepositoryLayer.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Repositories
{
	/// <summary>
	/// класс менеджера репозиториев проекта
	/// </summary>
	public class RepositoryManager : IRepositoryManager
	{
		private readonly Lazy<IUsersRepository> _lazyUsersRepository;

		private readonly Lazy<IPasswordChangesRepository> _lazyPasswordChangesRepository;
		private readonly Lazy<IFileDataRepository> _lazyFileDataRepository;
		private readonly Lazy<IPostsRepository> _lazyPostsRepository;
		private readonly Lazy<IPostFilesRepository> _lazyPostFilesRepository;
		private readonly Lazy<IHashTagRepository> _lazyHashTagRepository;
		private readonly Lazy<ICommentRepository> _lazyCommentRepository;
		private readonly Lazy<IAppealRepository> _lazyAppealRepository;
		private readonly Lazy<IRoleRepository> _lazyRoleRepository;
		private readonly Lazy<IUserRolesRepository> _lazyUserRolesRepository;

		public RepositoryManager(AHUBContext dbContext)
		{
			_lazyUsersRepository = new Lazy<IUsersRepository>(() => new  UsersRepository(dbContext));
			_lazyPasswordChangesRepository = new Lazy<IPasswordChangesRepository>(() => new PasswordChangesRepository(dbContext));
			_lazyFileDataRepository = new Lazy<IFileDataRepository>(() => new FileDataRepository(dbContext));
			_lazyPostsRepository = new Lazy<IPostsRepository>(() => new PostsRepository(dbContext));
			_lazyPostFilesRepository = new Lazy<IPostFilesRepository>(() => new PostFilesRepository(dbContext));
			_lazyHashTagRepository = new Lazy<IHashTagRepository>(() => new HashTagRepository(dbContext));
			_lazyCommentRepository = new Lazy<ICommentRepository>(() => new CommentRepository(dbContext));
			_lazyAppealRepository = new Lazy<IAppealRepository>(() => new AppealRepository(dbContext));
			_lazyRoleRepository = new Lazy<IRoleRepository>(() => new RoleRepository(dbContext));
			_lazyUserRolesRepository = new Lazy<IUserRolesRepository>(() => new UserRolesRepository(dbContext));
		}

		public IUsersRepository _usersRepository => _lazyUsersRepository.Value;

		public IPasswordChangesRepository _passwordChangesRepository => _lazyPasswordChangesRepository.Value;
		public IFileDataRepository _fileDataRepository => _lazyFileDataRepository.Value;
		public IPostsRepository _postsRepository => _lazyPostsRepository.Value;
		public IPostFilesRepository _postFilesRepository => _lazyPostFilesRepository.Value;
		public IHashTagRepository _hashTagRepository=> _lazyHashTagRepository.Value;
		public ICommentRepository _commentRepository => _lazyCommentRepository.Value;
		public IAppealRepository _appealRepository => _lazyAppealRepository.Value;
		public IRoleRepository _roleRepository => _lazyRoleRepository.Value;
		public IUserRolesRepository _userRolesRepository => _lazyUserRolesRepository.Value;
	}
}

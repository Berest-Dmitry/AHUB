using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using RepositoryLayer.IRepositories;
using ServicesLayer.IServices;
using StackExchange.Redis;

namespace ServicesLayer.Services
{
	/// <summary>
	/// менеджер сервисов
	/// </summary>
	public class ServiceManager: IServiceManager
	{
		private readonly Lazy<IUserService> _lazyUserService;
		private readonly Lazy<ITokenService> _lazyTokenService;
		private readonly Lazy<IHashService> _lazyHashService;
		private readonly Lazy<ISmsService> _lazySmsService;
		private readonly Lazy<IPasswordRecoveryService> _lazyPasswordRecoveryService;
		private readonly Lazy<IFilesService> _lazyFilesService;
		private readonly Lazy<IPostsService> _lazyPostsService;
		private readonly Lazy<IPostFilesService> _lazyPostFilesService;
		private readonly Lazy<IHashTagService> _lazyHashTagService;
		private readonly Lazy<ICommentService> _lazyCommentService;
		private readonly Lazy<IAppealService> _lazyAppealService;
		private readonly Lazy<IRoleService> _lazyRoleService;
		private readonly Lazy<IElasticPostService> _lazyElasticPostService;
		private readonly Lazy<IIntegrationService> _lazyIntegrationService;

		public IUserService _userService => _lazyUserService.Value;

		public ITokenService _tokenService => _lazyTokenService.Value;

		public IHashService _hashService => _lazyHashService.Value;

		public ISmsService _smsService => _lazySmsService.Value;

		public IPasswordRecoveryService _passwordRecoveryService => _lazyPasswordRecoveryService.Value;

		public IFilesService _filesService => _lazyFilesService.Value;

		public IPostsService _postsService => _lazyPostsService.Value;

		public IPostFilesService _postFilesService => _lazyPostFilesService.Value;

		public IHashTagService _hashTagService => _lazyHashTagService.Value;

		public ICommentService _commentService => _lazyCommentService.Value;

		public IAppealService _appealService=> _lazyAppealService.Value;

		public IRoleService _roleService => _lazyRoleService.Value;

		public IElasticPostService _elasticPostService => _lazyElasticPostService.Value;

        public IIntegrationService _integrationService => _lazyIntegrationService.Value;

        public ServiceManager(IRepositoryManager repositoryManager, IConfiguration config,
			IConnectionMultiplexer connectionMultiplexer, IElasticClient elasticClient) {
			_lazyUserService = new Lazy<IUserService>(() => new UserService(repositoryManager, this));
			_lazyTokenService = new Lazy<ITokenService>(() => new TokenService(repositoryManager));
			_lazyHashService = new Lazy<IHashService>(() => new HashService(repositoryManager, config));
			_lazySmsService = new Lazy<ISmsService> (() => new SmsService());
			_lazyPasswordRecoveryService = new Lazy<IPasswordRecoveryService>(() => new PasswordRecoveryService(repositoryManager, this));
			_lazyFilesService = new Lazy<IFilesService>(() => new FilesService(repositoryManager, config));
			_lazyPostsService = new Lazy<IPostsService>(() => new PostsService(repositoryManager, config, this));
			_lazyPostFilesService = new Lazy<IPostFilesService>(() => new PostFilesService(repositoryManager, this));
			_lazyHashTagService = new Lazy<IHashTagService>(() => new HashTagService(repositoryManager));
			_lazyCommentService = new Lazy<ICommentService>(() => new CommentService(repositoryManager, config, this));
			_lazyAppealService = new Lazy<IAppealService>(() => new AppealService(repositoryManager, this));
			_lazyRoleService = new Lazy<IRoleService>(() => new RoleService(repositoryManager, this));
			_lazyElasticPostService = new Lazy<IElasticPostService>(() => new ElasticPostService(elasticClient, repositoryManager, config));
			_lazyIntegrationService = new Lazy<IIntegrationService>(() => new IntegrationService(repositoryManager, config));
		}
	}
}

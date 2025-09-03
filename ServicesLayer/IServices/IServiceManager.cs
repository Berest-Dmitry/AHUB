using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.IServices
{
	/// <summary>
	/// интерфейс менеджера сервисов
	/// </summary>
	public interface IServiceManager
	{
		IUserService _userService { get; }

		ITokenService _tokenService { get; }

		IHashService _hashService { get; }

		ISmsService _smsService { get; }

		IPasswordRecoveryService _passwordRecoveryService { get; }

		IFilesService _filesService { get; }

		IPostsService _postsService { get; }

		IPostFilesService _postFilesService { get; }

		IHashTagService _hashTagService { get; }

		ICommentService _commentService { get; }

		IAppealService _appealService { get; }

		IRoleService _roleService { get; }

		IElasticPostService _elasticPostService { get; }

		IIntegrationService _integrationService { get; }

	}
}

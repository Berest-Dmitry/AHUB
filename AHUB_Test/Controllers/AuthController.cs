using ContractsLayer.Base;
using ContractsLayer.Dtos.Endpoints;
using ContractsLayer.Dtos;
using ContractsLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ServicesLayer.IServices;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace AHUB_Test.Controllers
{
	/// <summary>
	/// контроллер для восстановления паролей пользователей
	/// </summary>
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IServiceManager _serviceManager;
		private readonly IConfiguration _configuration;

		public AuthController(IServiceManager serviceManager, IConfiguration configuration)
		{
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}


		/// <summary>
		/// отправка смс-сообщения с кодом подтверждения смены пароля
		/// </summary>
		/// <param name="phoneNumber"></param>
		/// <param name="userName"></param>
		/// <returns></returns>
		/// <response code="200"></response>
		[HttpPost]
		[Route("send-sms")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<BaseModel>))]
		public async Task<BaseResponseModel<SecurityTokenCreateModel>> SendSmsMessage(string phoneNumber, string userName)
		{
			return await _serviceManager._passwordRecoveryService.GenerateKeyAndSendToUser(_configuration, phoneNumber, userName);
		}

		/// <summary>
		/// проверка кода подтверждения
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		/// <response code="200"></response>
		[HttpPost]
		[Route("enter-key")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<BaseModel>))]
		public async Task<BaseResponseModel<BaseModel>> ValidateRecoveryKey(string userName, string key, string token)
		{
			return await _serviceManager._passwordRecoveryService.ValidateRecoveryKey(userName, key, token);
		}

		/// <summary>
		/// смена пароля пользователя
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="newPassword"></param>
		/// <returns></returns>
		/// <response code="200"></response>
		[HttpPost]
		[Route("change-password")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<BaseModel>))]
		public async Task<BaseResponseModel<BaseModel>> ChangePassword(string userName, string newPassword)
		{
			return await _serviceManager._passwordRecoveryService.UpdatePassword(userName, newPassword);
		}

		/// <summary>
		/// вход в систему
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		/// <response code="200"></response>
		[HttpPost]
		[Route("login-user")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<UserLoginModel>))]
		public async Task<BaseResponseModel<UserLoginModel>> LoginUser(string username, string password)
		{
			return await _serviceManager._userService.LoginUser(HttpContext, _configuration, username, password);
		}

		/// <summary>
		/// регистрация пользователя в системе
		/// </summary>
		/// <param name="userInfo"></param>
		/// <returns></returns>
		/// <response code="200"></response>
		[HttpPost]
		[Route("register-user")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<UserDto>))]
		public async Task<BaseResponseModel<UserDto>> RegisterUser([FromBody] UserRegisterDto userInfo)
		{
			return await _serviceManager._userService.RegisterUser(userInfo);

		}

		/// <summary>
		/// обновление токена доступа
		/// </summary>
		/// <param name="tokenModel"></param>
		/// <returns></returns>
		/// <response code="200"></response>
		[HttpPost]
		[Route("refresh-token")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<UserLoginModel>))]
		public async Task<BaseResponseModel<UserLoginModel>> RefreshToken([FromBody] TokenModel tokenModel)
		{
			return await _serviceManager._userService.RefreshToken(_configuration, tokenModel);
		}
	}
}

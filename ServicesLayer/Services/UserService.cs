using RepositoryLayer.IRepositories;
using ServicesLayer.IServices;
using Microsoft.AspNetCore.Http;
using ContractsLayer.Base;
using ContractsLayer.Dtos;
using DomainLayer.Models;
using ContractsLayer.Common;
using ContractsLayer.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using ContractsLayer.Dtos.Endpoints;

namespace ServicesLayer.Services
{
    /// <summary>
    /// сервис пользователей
    /// </summary>
    public class UserService : IUserService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly IServiceManager _serviceManager;

		public UserService(IRepositoryManager repositoryManager, IServiceManager serviceManager)
		{
			_repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager)) ;
		}

		/// <summary>
		/// метод входа в систему
		/// </summary>
		/// <param name="httpContext"></param>
		/// <param name="username"></param>
		/// <param name="refreshToken"></param>
		/// <returns></returns>
		public async Task<BaseModel> StartSession(HttpContext httpContext,string username, string refreshToken)
		{
			try
			{
				if(httpContext != null)
				{
					httpContext.Session.SetString(username, refreshToken);
					await httpContext.Session.CommitAsync();
					return new BaseModel();
				}
				else
				{
					return new BaseModel(new Exception("An error occured while retrieving http context"));
				}
			}
			catch (Exception ex) { 
				return BaseModelUtilities<BaseModel>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод регистрации пользователя
		/// </summary>
		/// <returns></returns>
		public async Task<BaseResponseModel<UserDto>> RegisterUser(UserRegisterDto userData)
		{
			try
			{
				var userEntry = new User
				{
					firstName = userData.firstName,
					lastName = userData.lastName,
					userName = userData.userName,
				};

				// проверка на существование пользователя с таким логином 
				var existingUser = await _repositoryManager._usersRepository.GetUserByUserName(userData.userName);
				if(existingUser != null)
				{
					return new BaseResponseModel<UserDto>
					{
						Result = DefaultEnums.Result.error,
						Entity = null,
						Error = new Exception("User with the same login already exists!")
					};
				}

				string hash = string.Empty;
				byte[] salt = null;
				hash = _serviceManager._hashService.HashPassword(userData.password, out salt);
				userEntry.hashedPassword = hash;
				userEntry.salt = BitConverter.ToString(salt);

				var userProps = ObjectManager<User>.GetListOfObjectPropertyNames(userEntry);
				var registrationRes = await _repositoryManager._usersRepository.CreateOrUpdateUserEntry(userEntry, userProps);
				if (registrationRes != null)
				{
					// добавление роли "Пользователь" только что созданному пользователю
					var defaultRoleId = await _repositoryManager._roleRepository.GetDefaultUserRole();
					if (defaultRoleId == Guid.Empty)
					{
						throw new Exception("An error occurred while retrieving default user role");

					}

					var userRole = await _repositoryManager._userRolesRepository.CreateUserRolesEntry(registrationRes.id, defaultRoleId);
					if(userRole == null)
					{
						throw new Exception("Failed to add base role to created user");
					}

					var mappedEntity = ObjectMapper.Mapper.Map<UserDto>(registrationRes);
					return new BaseResponseModel<UserDto>
					{
						Result = DefaultEnums.Result.ok,
						Entity = mappedEntity
					};
				}
				else {
					return new BaseResponseModel<UserDto>
					{
						Result = DefaultEnums.Result.error,
						Entity = null,
						Error = new Exception("An error occurred while creating user entry!")
					};
				}
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<UserDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод обновления данных о пользователе
		/// </summary>
		/// <param name="userInfo"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<UserDto>> UpdateUserData(UserUpdateDto userInfo)
		{
			try
			{
				var currentUser = await _repositoryManager._usersRepository.GetByIdAsync(userInfo.id);
				if(currentUser == null)
				{
					return new BaseResponseModel<UserDto>
					{
						Result = DefaultEnums.Result.error,
						Entity = null,
						Error = new Exception("User with current ID doesn't exist!")
					};
				}

				var propsToUpdate = ObjectManager<UserUpdateDto>.GetListOfObjectPropertyNames(userInfo);

				var userData = ObjectMapper.Mapper.Map<User>(userInfo);
				var updateRes = await _repositoryManager._usersRepository.CreateOrUpdateUserEntry(userData, propsToUpdate);
				if (updateRes != null)
				{
					var mappedEntity = ObjectMapper.Mapper.Map<UserDto>(updateRes);
					return new BaseResponseModel<UserDto>
					{
						Result = DefaultEnums.Result.ok,
						Entity = mappedEntity
					};
				}
				else
				{
					return new BaseResponseModel<UserDto>
					{
						Result = DefaultEnums.Result.error,
						Entity = null,
						Error = new Exception("An error occurred while updating user entry!")
					};
				}
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<UserDto>>.ErrorFormat(ex);

			}
		}

		/// <summary>
		/// метод выхода из системы
		/// </summary>
		/// <param name="httpContext"></param>
		/// <param name="username"></param>
		/// <returns></returns>
		public async Task<BaseModel> EndSession(HttpContext httpContext, string username)
		{
			try
			{
				if(httpContext == null)
				{
					return new BaseModel(new Exception("An error occured while retrieving http context"));
				}
				else
				{
					httpContext.Session.Remove(username);
					await httpContext.Session.CommitAsync();
					return new BaseModel();
				}
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseModel>.ErrorFormat(ex);
			}
		}
		
		/// <summary>
		/// метод авторизации в системе
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<UserLoginModel>> LoginUser(HttpContext httpContext, IConfiguration _config, string username, string password)
		{
			try
			{
				var existingUser = await GetUserOrFail(username, password);
				if(existingUser?.Result == DefaultEnums.Result.error)
				{
					return new BaseResponseModel<UserLoginModel>(existingUser?.Error);
				}

				var tokenCreateRes = await _serviceManager._tokenService.WriteAccessToken(_config, existingUser.Entity);
				if (tokenCreateRes.Result == DefaultEnums.Result.error)
				{
					return new BaseResponseModel<UserLoginModel>(new Exception("Authentication failed! Additional info: " + tokenCreateRes?.ErrorInfo));
				}
				var refershTokenCreateRes =  _serviceManager._tokenService.WriteRefreshToken();
				if(refershTokenCreateRes.Result == DefaultEnums.Result.error)
				{
					return new BaseResponseModel<UserLoginModel>(new Exception("Authentication failed! Additional info: " + refershTokenCreateRes?.ErrorInfo));
				}
				var sessionCreateRes = await StartSession(httpContext, username, refershTokenCreateRes.Entity);
				if ( sessionCreateRes.Result == DefaultEnums.Result.ok)
				{
					var expirationTimeInHours = Convert.ToInt16(_config["Jwt:RefreshTokenExpirationTime"]);
					var loginModel = new UserLoginModel
					{
						accessToken = tokenCreateRes.Entity,
						refreshToken = refershTokenCreateRes.Entity,
						RefreshTokenExpiryTime = DateTime.Now.AddHours(expirationTimeInHours)
					};
					return new BaseResponseModel<UserLoginModel>(loginModel);
				}
				else return new BaseResponseModel<UserLoginModel>(sessionCreateRes.Error);
			}
			catch(Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<UserLoginModel>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод проверки существования пользователя с данными логином и паролем
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<User>> GetUserOrFail(string userName, string password)
		{
			try
			{
				var existingUser = await _repositoryManager._usersRepository.GetUserByUserName(userName);
                if (existingUser == null)
                {
					throw new Exception("The user with such login wasn't found!");
                }
				var salt = _serviceManager._hashService.GetByteArrayFromUTF8String(existingUser.salt, '-');
				bool passwordCheckRes = _serviceManager._hashService.VerifyPassword(password, existingUser.hashedPassword, salt);
				if (!passwordCheckRes)
				{
					throw new Exception("The password is incorrect!");
				}
				return new BaseResponseModel<User>
				{
					Result = DefaultEnums.Result.ok,
					Entity = existingUser
				};
			}
			catch(Exception ex)
			{
				return new BaseResponseModel<User>
				{
					Result = DefaultEnums.Result.error,
					Error = ex
				};
			}
		}

		/// <summary>
		/// метод обновления токена доступа пользователя
		/// </summary>
		/// <param name="_config"></param>
		/// <param name="tokenModel"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<UserLoginModel>> RefreshToken(IConfiguration _config, TokenModel tokenModel)
		{
			try
			{
				var principal = await _serviceManager._tokenService.GetPrincipalFromToken(_config, tokenModel.accessToken);
				if( principal.Result == DefaultEnums.Result.error)
				{
					return new BaseResponseModel<UserLoginModel>(principal != null ? principal.Error : new Exception("An error occured while retrieving user's credentials"));
				}

				var userId = _serviceManager._tokenService.GetClaimByName("user_id", principal?.Entity?.Claims);
				
				var newAccessTokenCreateRes = await _serviceManager._tokenService.CheckByIdAndWriteAccessToken(_config, new Guid(userId));
				if( newAccessTokenCreateRes.Result == DefaultEnums.Result.error)
				{
					return new BaseResponseModel<UserLoginModel>
					{
						Result = DefaultEnums.Result.error,
						Error = newAccessTokenCreateRes?.Error
					};
				}
				var newRefreshTokenCreateRes = _serviceManager._tokenService.WriteRefreshToken();
				if( newRefreshTokenCreateRes.Result == DefaultEnums.Result.error)
				{
					return new BaseResponseModel<UserLoginModel>
					{
						Result = DefaultEnums.Result.error,
						Error = newRefreshTokenCreateRes.Error
					};
				}
				var expirationTimeInHours = Convert.ToInt16(_config["Jwt:RefreshTokenExpirationTime"]);
				var loginModel = new UserLoginModel
				{
					accessToken = newAccessTokenCreateRes.Entity,
					refreshToken = newRefreshTokenCreateRes.Entity,
					RefreshTokenExpiryTime = DateTime.Now.AddHours(expirationTimeInHours)
				};
				return new BaseResponseModel<UserLoginModel>(loginModel);
			}
			catch(Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<UserLoginModel>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод пометки пользователя удаленным
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<UserDto>> RemoveUser(Guid userId)
		{
			try
			{
				var delResult = await _repositoryManager._usersRepository.DeleteUserEntry(userId);
				if(delResult != null)
				{
					var userDto = ObjectMapper.Mapper.Map<UserDto>(delResult);
					return new BaseResponseModel<UserDto>(userDto);
				}
				else return new BaseResponseModel<UserDto> 
				{ 
					Result = DefaultEnums.Result.error,
					Error = new Exception("An error occurred while removing user entry")
				};
			}
			catch(Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<UserDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод получения логина пользователя из JWT-токена
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<string>> GetUserNameFromToken(HttpContext ctx, IConfiguration config)
		{
			try
			{
				var token = await CommonUtilities.ReadJWTToken(ctx);
				var principal = await _serviceManager._tokenService.GetPrincipalFromToken(config, token);
				if (principal.Result == DefaultEnums.Result.error)
				{
					return new BaseResponseModel<string>(principal != null
						? principal.Error
						: new Exception("An error occured while retrieving user's credentials"));
				}

				var userName = _serviceManager._tokenService.GetClaimByName("user_name", principal?.Entity?.Claims);
				return new BaseResponseModel<string>(userName);
			}
			catch (Exception ex)
			{
				return new BaseResponseModel<string>(ex);
			}
		}

		/// <summary>
		/// получение инф-ии о текущем пользователе, вошедшем в систему (по логину)
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<UserDto>> GetCurrentUser(string userName)
		{
			return await GetUserOrThrowError(userName);
		}

		/// <summary>
		/// получение инф-ии о текущем пользователе, вошедшем в систему (по ID)
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<UserDto>> GetCurrentUser(Guid userId)
		{
			return await GetUserOrThrowError(userId:  userId);
		}

		/// <summary>
		/// метод получения информации о выбранном пользователе
		/// </summary>
		/// <param name="userName">логин</param>
		/// <returns>возвращает пользователя либо сообщение об ошибке, если пользователь не найден</returns>
		private async Task<BaseResponseModel<UserDto>> GetUserOrThrowError(string? userName = null, Guid? userId = null)
		{
			try
			{
				User user = null;
				if(userName != null)
				{
					user = await _repositoryManager._usersRepository.GetUserByUserName(userName);
				}
				else if(userId != null)
				{
					user = await _repositoryManager._usersRepository.GetUserById(userId ?? Guid.Empty);
				}
				if (user != null)
				{
					var userModel = ObjectMapper.Mapper.Map<UserDto>(user);
					return new BaseResponseModel<UserDto>
					{
						Entity = userModel,
						Result = DefaultEnums.Result.ok
					};
				}
				else
				{
					return new BaseResponseModel<UserDto>
					{
						Result = DefaultEnums.Result.error,
						Entity = null,
						Error = new Exception("User with this login wasn't found!")
					};
				}
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<UserDto>>.ErrorFormat(ex);
			}
		}
	}
}

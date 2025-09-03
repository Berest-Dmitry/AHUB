using ContractsLayer.Base;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using ContractsLayer.Models;
using DomainLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ServicesLayer.IServices
{
    /// <summary>
    /// интерфейс сервиса пользователей
    /// </summary>
    public interface IUserService
	{ 
		Task<BaseModel> StartSession(HttpContext httpContext,string username, string refreshToken);

		Task<BaseResponseModel<UserDto>> RegisterUser(UserRegisterDto userData);

		Task<BaseResponseModel<UserDto>> UpdateUserData(UserUpdateDto userInfo);

		Task<BaseModel> EndSession(HttpContext httpContext, string username);

		Task<BaseResponseModel<UserLoginModel>> LoginUser(HttpContext httpContext, IConfiguration _config, string username, string password);

		Task<BaseResponseModel<UserLoginModel>> RefreshToken( IConfiguration _config, TokenModel tokenModel);

		Task<BaseResponseModel<User>> GetUserOrFail(string userName, string password);

		Task<BaseResponseModel<UserDto>> RemoveUser(Guid userId);

		Task<BaseResponseModel<UserDto>> GetCurrentUser(string userName);

		Task<BaseResponseModel<UserDto>> GetCurrentUser(Guid userId);

		Task<BaseResponseModel<string>> GetUserNameFromToken(HttpContext ctx, IConfiguration config);

	}
}

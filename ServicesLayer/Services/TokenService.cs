using ContractsLayer.Base;
using ContractsLayer.Common;
using DomainLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.IRepositories;
using ServicesLayer.IServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Services
{
	public class TokenService: ITokenService
	{
		private readonly IRepositoryManager _repositoryManager;

		public TokenService(IRepositoryManager repositoryManager)
		{
			_repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
		}

		/// <summary>
		/// метод создания Jwt-токена 
		/// </summary>
		/// <param name="_config">объект настроек системы</param>
		/// <param name="user">сущность пользователя</param>
		/// <returns>возвращает модель результата создания токена</returns>
		public async Task<BaseResponseModel<string>> WriteAccessToken(IConfiguration _config, User user)
		{
			try
			{
				var userRoles = await _repositoryManager._userRolesRepository.GetListOfUserRoles(user.id);
				var result = CreateAccessToken(_config, user, userRoles);
				return new BaseResponseModel<string>
				{
					Result = DefaultEnums.Result.ok,
					Entity = result
				};
			}
			catch(Exception ex) {
				return new BaseResponseModel<string>
				{
					Result = DefaultEnums.Result.error,
					Entity = null,
					Error = ex
				};
			}
		}

		/// <summary>
		/// метод создания Jwt-токена (с проверкой валидности данных пользователя по коду пользователя)
		/// </summary>
		/// <param name="_config"></param>
		/// <param name="id"></param>
		/// <returns>возвращает модель результата создания токена</returns>
		public async Task<BaseResponseModel<string>> CheckByIdAndWriteAccessToken(IConfiguration _config, Guid id)
		{
			try
			{
				var user = await _repositoryManager._usersRepository.GetByIdAsync(id);
				if (user == null)
				{
					return new BaseResponseModel<string>
					{
						Result = DefaultEnums.Result.error,
						Entity = null,
						Error = new Exception("An error occurred while searching for user data")
					};
				}
				else
				{
					var userRoles = await _repositoryManager._userRolesRepository.GetListOfUserRoles(user.id);
					var result = CreateAccessToken(_config, user, userRoles);
					return new BaseResponseModel<string>
					{
						Result = DefaultEnums.Result.ok,
						Entity = result
					};
				}
			}
			catch (Exception ex)
			{
				return new BaseResponseModel<string>
				{
					Result = DefaultEnums.Result.error,
					Entity = null,
					Error = ex
				};
			}
		}

		/// <summary>
		/// метод создания Jwt-токена 
		/// </summary>
		/// <returns></returns>
		private string CreateAccessToken(IConfiguration _config, User user, List<Role> userRoles)
		{
			try
			{
				// создание списка пользоательских рекламаций
				var totalClaims = new List<Claim>();
				var claims = new[]
				{
						new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
						new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
						new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
						new Claim("user_id", user?.id.ToString()),
						new Claim("display_name", user?.firstName + user?.lastName),
						new Claim("user_name", user?.userName),
					};

				// добавление ролей пользователя к пользовательским рекламациям
				var roleClaims = new List<Claim>();
				foreach (var role in userRoles)
				{
					roleClaims.Add(new Claim(ClaimTypes.Role, role.name));
				}
				totalClaims.AddRange(claims);
				if(roleClaims.Count > 0)
				{
					totalClaims.AddRange(roleClaims);
				}

				// создание токена JWT
				var expirationTimeInHours = Convert.ToInt16(_config["Jwt:RefreshTokenExpirationTime"]);
				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTKey"]));
				var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
				var token = new JwtSecurityToken(
					_config["Jwt:Issuer"],
					_config["Jwt:Audience"],
					totalClaims.ToArray(),
					expires: DateTime.Now.AddHours(expirationTimeInHours), signingCredentials: signIn);

				// создание обработчика токенов и обработка
				var tokenHandler = new JwtSecurityTokenHandler();
				var result = tokenHandler.WriteToken(token);
				return result;
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// метод создания токена обновления
		/// </summary>
		/// <returns></returns>
		public BaseResponseModel<string> WriteRefreshToken()
		{
			try
			{
				var randomNumber = new byte[32];
				using (var rng = RandomNumberGenerator.Create())
				{
					rng.GetBytes(randomNumber);
					return new BaseResponseModel<string>
					{
						Result = DefaultEnums.Result.ok,
						Entity = Convert.ToBase64String(randomNumber)
					};
				}
			}
			catch(Exception ex)
			{
				return new BaseResponseModel<string>
				{
					Result = DefaultEnums.Result.error,
					Error = ex
				};
			}
		}

		/// <summary>
		/// метод получения пользовательских данных по токену доступа
		/// </summary>
		/// <param name="_config"></param>
		/// <param name="token"></param>
		/// <returns>возвращает модель ответа с пользовательскими данными</returns>
		public async Task<BaseResponseModel<ClaimsPrincipal>> GetPrincipalFromToken(IConfiguration _config, string token)
		{
			try
			{
				var tokenValidationParams = new TokenValidationParameters
				{
					ValidateAudience = false,
					ValidateIssuer = false,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTKey"])),
					ValidateLifetime = false
				};

				var tokenHandler = new JwtSecurityTokenHandler();
				var principal = tokenHandler.ValidateToken(token, tokenValidationParams, out SecurityToken securityToken);
				var jwtSecurityToken = securityToken as JwtSecurityToken;

				if(jwtSecurityToken == null || 
					!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
				{
					throw new SecurityTokenException("Invalid token!");
				}
				return new BaseResponseModel<ClaimsPrincipal>
				{
					Result = DefaultEnums.Result.ok,
					Entity = principal
				};
			}
			catch(Exception ex)
			{
				return new BaseResponseModel<ClaimsPrincipal> { Result = DefaultEnums.Result.error, Error = ex };
			}
		}

		/// <summary>
		/// получение значения реквизита пользователя по его названию
		/// </summary>
		/// <param name="claimName">название реквизита</param>
		/// <param name="claims">список реквизитов</param>
		/// <returns></returns>
		public string GetClaimByName(string claimName, IEnumerable<Claim> claims)
		{
			return claims.Where(c => c.Type == claimName).FirstOrDefault()?.Value;
		}
	}
}

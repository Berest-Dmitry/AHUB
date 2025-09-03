using ContractsLayer.Base;
using DomainLayer.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace ServicesLayer.IServices
{
	public interface ITokenService
	{
		Task<BaseResponseModel<string>> WriteAccessToken(IConfiguration _config, User user);

		Task<BaseResponseModel<string>> CheckByIdAndWriteAccessToken(IConfiguration _config, Guid id);

		BaseResponseModel<string> WriteRefreshToken();

		Task<BaseResponseModel<ClaimsPrincipal>> GetPrincipalFromToken(IConfiguration _config, string token);

		string GetClaimByName(string claimName, IEnumerable<Claim> claims);
	}
}

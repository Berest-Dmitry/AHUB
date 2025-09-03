using ContractsLayer.Base;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using DomainLayer.Models;
using Microsoft.Extensions.Configuration;

namespace ServicesLayer.IServices
{
	/// <summary>
	/// интерфейс сервиса восстановления пароля
	/// </summary>
	public interface IPasswordRecoveryService
	{
		//int GenerateSecurityRecoveryKey();

		Task<BaseResponseModel<SecurityTokenCreateModel>> GenerateKeyAndSendToUser(IConfiguration _config, string userPhoneNumber, string userName);

		Task<PasswordChangesDto> SavePasswordChangesEntry(User  user, int? recoveryKey = null, string? recoveryToken = null, PasswordChanges passwordChangesEntry = null);

		Task<BaseResponseModel<BaseModel>> ValidateRecoveryKey(string userName, string recoveryKey, string recoveryToken);

		Task<BaseResponseModel<BaseModel>> UpdatePassword(string userName, string newPassword);
	}
}

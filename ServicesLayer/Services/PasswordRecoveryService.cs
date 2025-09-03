using ContractsLayer.Base;
using ContractsLayer.Common;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using ContractsLayer.Models;
using DomainLayer.Models;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.IRepositories;
using ServicesLayer.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Services
{
	/// <summary>
	/// сервис восстановления паролей пользователей
	/// </summary>
	public class PasswordRecoveryService: IPasswordRecoveryService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly IServiceManager _serviceManager;
		private readonly int minValue = 100000;
		private readonly int maxValue = 999999;
		private readonly string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz_0123456789";

		public PasswordRecoveryService(IRepositoryManager repositoryManager, IServiceManager serviceManager)
		{
			_repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
		}

		/// <summary>
		/// метод генерации случайного числа для восстановления пароля
		/// </summary>
		/// <returns></returns>
		private int GenerateSecurityRecoveryKey()
		{
			return RandomNumberGenerator.GetInt32(minValue, maxValue);
		}

		/// <summary>
		/// метод генерации случайного токена для восстановления пароля
		/// </summary>
		/// <returns></returns>
		private string GenerateSecurityRecoveryToken()
		{
			int start = 0, end = Alphabet.Length;
			string result = "";
			var random = new Random();
			for(int i = 0; i < 20; i++)
			{
				int nextInt = random.Next(start, end);
				result += Alphabet[nextInt];
			}
			return result;
		}

		/// <summary>
		/// метод отправки сгенерированного кода восстановления пароля
		/// смс-сообщением пользователю
		/// </summary>
		/// <param name="_config"></param>
		/// <param name="userPhoneNumber"></param>
		/// <param name="userName"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<SecurityTokenCreateModel>> GenerateKeyAndSendToUser(IConfiguration _config, string userPhoneNumber, string userName)
		{
			try
			{
				var currentUser = await _repositoryManager._usersRepository.GetUserByUserName(userName);
				if (currentUser == null)
				{
					return new BaseResponseModel<SecurityTokenCreateModel>(new Exception("User wasn't found"));
				}

				var smsServerUrl = _config["SmsServer:Url"];
				var smsApiId = _config["SmsApiId"];
				var testMode = _config["SmsServer:TestMode"];
				var recoveryKey = GenerateSecurityRecoveryKey();
				var securityRecoveryToken = new SecurityTokenCreateModel( GenerateSecurityRecoveryToken());

				var smsSendingModel = new SmsSendingModel();
				smsSendingModel.BaseSmsServerURL = smsServerUrl;
				smsSendingModel.MessageContent = recoveryKey.ToString();
				smsSendingModel.SmsRuApiId = smsApiId;
				smsSendingModel.Addressee = userPhoneNumber;
				smsSendingModel.TestMode = Convert.ToInt32(testMode);

				var result = await _serviceManager._smsService.SendMessage(smsSendingModel);
				if(result)
				{
					var savePasswordChangesRes = await SavePasswordChangesEntry(currentUser, recoveryKey, securityRecoveryToken.PasswordRecoveryToken);
					if (savePasswordChangesRes != null)
					{
						return new BaseResponseModel<SecurityTokenCreateModel>(securityRecoveryToken);
					}
					else return new BaseResponseModel<SecurityTokenCreateModel>
					{
						Result = DefaultEnums.Result.error,
						Error = new Exception("An error occurred while saving user's password recovery data")
					};
				}
				else return new BaseResponseModel<SecurityTokenCreateModel> { 
					Result = DefaultEnums.Result.error,
					Error = new Exception("An error occurred while sending message to user")
				};

			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<SecurityTokenCreateModel>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод сохранения записи об изменении пароля пользователя
		/// </summary>
		/// <param name="user">инф-я о пользователе</param>
		/// <param name="recoveryKey">смс-код подтверждения</param>
		/// <param name="recoveryToken">токен безопасности для смены пароля</param>
		/// <param name="passwordChangesEntry">запись в БД о смене пароля пользователем</param>
		/// <returns></returns>
		public async Task<PasswordChangesDto> SavePasswordChangesEntry(User user, int? recoveryKey = null, string? recoveryToken = null
			, PasswordChanges passwordChangesEntry = null)
		{
			try
			{
				PasswordChanges result = null;
				if(passwordChangesEntry == null)
				{
					var newEntry = new PasswordChanges()
					{
						userId = user?.id ?? Guid.Empty,
						recoveryKey = recoveryKey.Value,
						recoveryToken = recoveryToken,
						keyValidBefore = DateTime.UtcNow.AddMinutes(10),
					};
					result = await _repositoryManager._passwordChangesRepository.AddAsync(newEntry);
				}
				else
				{
					if(passwordChangesEntry.changePasswordBefore == null)
					{
						passwordChangesEntry.changePasswordBefore = DateTime.UtcNow.AddMinutes(10);
					}
					else
					{
						passwordChangesEntry.passwordChangeTime = DateTime.UtcNow;
					}
					result = await _repositoryManager._passwordChangesRepository.UpdateAsync(passwordChangesEntry);
				}
				if (result != null)
				{
					return ObjectMapper.Mapper.Map<PasswordChangesDto>(result);
				}
				else return null;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// метод проверки ключа восстановления
		/// </summary>
		/// <param name="userName">логин пользователя</param>
		/// <param name="recoveryKey">код восстановления из СМС</param>
		/// <param name="recoveryToken">токен восстановления, сгенерированный сервером</param>
		/// <returns></returns>
		public async Task<BaseResponseModel<BaseModel>> ValidateRecoveryKey(string userName, string recoveryKey, string recoveryToken)
		{
			try
			{
				var currentUser = await _repositoryManager._usersRepository.GetUserByUserName(userName);
				if (currentUser == null)
				{
					return new BaseResponseModel<BaseModel>(new Exception("User wasn't found"));
				}

				int currKey = Convert.ToInt32(recoveryKey);
				var passwordRecoveryEntry = await _repositoryManager
					._passwordChangesRepository
					.GetAsync(p => p.userId == currentUser.id 
					&& p.recoveryKey == currKey 
					&& p.recoveryToken.Equals(recoveryToken)
					&& DateTime.UtcNow < p.keyValidBefore
					&& p.changePasswordBefore == null && p.passwordChangeTime == null);

                if (passwordRecoveryEntry == null || passwordRecoveryEntry.Count == 0)
                {
					return new BaseResponseModel<BaseModel>(new Exception("Recovery key didn't pass validation!"));
                }
				else
				{
					var passwordChanges = passwordRecoveryEntry.FirstOrDefault();
					var updateRes = await SavePasswordChangesEntry(currentUser, passwordChangesEntry: passwordChanges);
					if (updateRes == null)
					{
						return new BaseResponseModel<BaseModel>(new Exception("An error occurred while saving user's password recovery data"));
					}
					else return new BaseResponseModel<BaseModel>();
				}
            }
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<BaseModel>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод смены пароля пользователя
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="newPassword"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<BaseModel>> UpdatePassword(string userName, string newPassword)
		{
			try
			{
				if(newPassword.Length < 7 || newPassword.Length > 25)
				{
					return new BaseResponseModel<BaseModel>(new Exception("Incorrect password!"));
				}

				var currentUser = await _repositoryManager._usersRepository.GetUserByUserName(userName);
				if (currentUser == null)
				{
					return new BaseResponseModel<BaseModel>(new Exception("User wasn't found"));
				}

				var passwordRecoveryEntry = await _repositoryManager
					._passwordChangesRepository
					.GetAsync(p => p.userId == currentUser.id && p.passwordChangeTime == null
						&& p.changePasswordBefore != null && DateTime.UtcNow < p.changePasswordBefore);

				if (passwordRecoveryEntry == null || passwordRecoveryEntry.Count == 0)
				{
					return new BaseResponseModel<BaseModel>(new Exception("Password change failed!"));
				}

				byte[] salt = null;
				var newHashedPassword = _serviceManager._hashService.HashPassword(newPassword, out salt);
				var saltStr = BitConverter.ToString(salt);

				var updateRes = await _repositoryManager._usersRepository.UpdatePassword(currentUser, newHashedPassword, saltStr);
				if (!updateRes)
				{
					return new BaseResponseModel<BaseModel>(new Exception("An error occured while saving new password!"));
				}
				else
				{
					var passwordChanges = passwordRecoveryEntry.FirstOrDefault();
					var updPasswordChangesRes = await SavePasswordChangesEntry(currentUser, passwordChangesEntry: passwordChanges);
					if (updPasswordChangesRes == null)
					{
						return new BaseResponseModel<BaseModel>(new Exception("An error occurred while saving user's password recovery data"));
					}
					else return new BaseResponseModel<BaseModel>();
				}
				
			}
			catch (Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<BaseModel>>.ErrorFormat(ex);
			}
		}
	}
}

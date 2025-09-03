
namespace ContractsLayer.Dtos.Endpoints
{
	/// <summary>
	/// модель возврата токена безопасности для восстановления пароля
	/// </summary>
	public class SecurityTokenCreateModel
	{
		/// <summary>
		/// токен безопасности для восстановления пароля
		/// </summary>
		public string PasswordRecoveryToken { get; set; }

		public SecurityTokenCreateModel(string passwordRecoveryToken)
		{
			PasswordRecoveryToken = passwordRecoveryToken;
		}
	}
}

using ContractsLayer.Base;
using System.ComponentModel.DataAnnotations;


namespace ContractsLayer.Dtos
{
	/// <summary>
	/// модель представления изменения пароля пользователя
	/// </summary>
	public class PasswordChangesDto: BaseEntityModel
	{
		/// <summary>
		/// код пользователя
		/// </summary>
		public Guid userId { get; set; }

		/// <summary>
		/// ключ восстановления (доступа)
		/// </summary>
		[Required]
		[MinLength(6)]
		[MaxLength(6)]
		public string recoveryKey { get; set; }

		/// <summary>
		/// защитный токен для смены пароля
		/// </summary>
		[Required]
		[MinLength(20)]
		[MaxLength(20)]
		public string recoveryToken { get; set; }

		/// <summary>
		/// дата и время, до которой действителен код смены пароля
		/// </summary>
		public DateTime? keyValidBefore { get; set; }

		/// <summary>
		/// дата и время, до которой пользователь может поменять пароль
		/// после подтверждения кода доступа
		/// </summary>
		public DateTime? changePasswordBefore { get; set; }

		/// <summary>
		/// дата и время замены пароля
		/// </summary>
		public DateTime? passwordChangeTime { get; set; }
	}
}

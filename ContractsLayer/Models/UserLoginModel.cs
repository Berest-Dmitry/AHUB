using ContractsLayer.Base;

namespace ContractsLayer.Models
{
	/// <summary>
	/// модель ответа на запрос о входе пользователя в систему
	/// </summary>
	public class UserLoginModel: BaseModel
	{
		public string accessToken { get; set; }
		public string refreshToken { get; set; }

		public DateTime RefreshTokenExpiryTime { get; set; }
	}
}

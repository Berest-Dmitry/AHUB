using System.ComponentModel.DataAnnotations;


namespace ContractsLayer.Models
{
	/// <summary>
	/// модель пары токенов (доступ-обновление),
	/// передаваемая при обновлении токена пользователя
	/// </summary>
	public class TokenModel
	{
		[Required]
		public string accessToken { get; set; }

		[Required]
		public string refreshToken {  get; set; }
	}
}


using Microsoft.Extensions.Configuration;

namespace ServicesLayer.IServices
{
	/// <summary>
	/// интерфейс сервиса хеширования
	/// </summary>
	public  interface IHashService
	{
		string HashPassword(string password, out byte[] salt);
		
		bool VerifyPassword(string password, string hash,  byte[] salt);

		byte[] GetByteArrayFromUTF8String(string input, char splitChar);
	}
}

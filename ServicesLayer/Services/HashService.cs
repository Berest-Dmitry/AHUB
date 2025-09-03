using Microsoft.Extensions.Configuration;
using RepositoryLayer.IRepositories;
using ServicesLayer.IServices;
using System.Security.Cryptography;
using System.Text;

namespace ServicesLayer.Services
{
	/// <summary>
	/// сервис хеширования паролей
	/// </summary>
	public class HashService : IHashService
	{
		private readonly IRepositoryManager _repositoryManager;

		private readonly IConfiguration _configuration;

		HashAlgorithmName algorithmName = HashAlgorithmName.SHA512;

		public HashService(IRepositoryManager repositoryManager, IConfiguration configuration)
		{
			_repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		/// <summary>
		/// метод хеширования пароля
		/// </summary>
		/// <param name="password"></param>
		/// <param name="salt"></param>
		/// <returns></returns>
		public string HashPassword(string password, out byte[] salt)
		{
			var keySize = Convert.ToInt16(_configuration["HashSettings:keySize"]);
			var iterations = Convert.ToInt32(_configuration["HashSettings:iterations"]);
			salt = RandomNumberGenerator.GetBytes(keySize);

			var hash = Rfc2898DeriveBytes.Pbkdf2(
				Encoding.UTF8.GetBytes(password),
				salt,
				iterations,
				algorithmName,
				keySize);

			return Convert.ToHexString(hash);
		}

		/// <summary>
		/// метод сравнения хешированного пароля и пароля,
		/// предоставленного пользователем
		/// </summary>
		/// <param name="password"></param>
		/// <param name="hash"></param>
		/// <param name="salt"></param>
		/// <returns></returns>
		public bool  VerifyPassword(string password, string hash, byte[] salt)
		{
			var keySize = Convert.ToInt16(_configuration["HashSettings:keySize"]);
			var iterations = Convert.ToInt32(_configuration["HashSettings:iterations"]);
			var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, algorithmName, keySize);

			return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
		}

		/// <summary>
		/// метод получения массива байт из строки в формате UTF-8
		/// </summary>
		/// <param name="input"></param>
		/// <param name="splitChar"></param>
		/// <returns></returns>
		public byte[] GetByteArrayFromUTF8String(string input, char splitChar)
		{
			string[] tempArr = input.Split(splitChar);
			byte[] result = new byte[tempArr.Length];
			for(int i = 0; i < tempArr.Length; i++)
			{
				result[i] = Convert.ToByte(tempArr[i], 16);
			}
			return result;
		}
	}
}

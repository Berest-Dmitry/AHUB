using Newtonsoft.Json;

namespace AHUB_Test.Config
{
	/// <summary>
	/// класс менеджера секретов приложения
	/// </summary>
	public static class SecretsManager
	{
		/// <summary>
		/// метод загрузки переменных окружения проекта
		/// </summary>
		/// <param name="filePath"></param>
		public static void Load(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return;
			}

			string text = File.ReadAllText(filePath);
			var settingsObject = JsonConvert.DeserializeObject<UserSecretsItem>(text, new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore,
			});			
			
			foreach(var prop in typeof(UserSecretsItem).GetProperties())
			{
				Environment.SetEnvironmentVariable(prop.Name, Convert.ToString(prop.GetValue(settingsObject)));
			}
			
		}

		/// <summary>
		/// объект секретных настроек проекта
		/// </summary>
		internal class UserSecretsItem
		{
			public string SmsApiId { get; set; }

			public string MinioSecret { get; set; }

			public string JWTKey { get; set; }

			public string ElasticPwd {  get; set; }

			public string ElasticLogin { get; set; }

			public string DbConnectionProd {  get; set; }

			public string DbConnectionDev { get; set; }

			public string ElasticCertificate { get; set; }
		}
	}

}

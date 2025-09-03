using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Xml.Serialization;

namespace ContractsLayer.Common
{
	/// <summary>
	/// класс, содержащий общеиспользуемые утилиты
	/// </summary>
	public static class CommonUtilities
	{
		/// <summary>
		/// метод чтения JWT-токена
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static async Task<string> ReadJWTToken(HttpContext context)
		{
			return await context.GetTokenAsync("access_token");
		}

        /// <summary>
        /// метод получения размера любого объекта, наследуемого от System.object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long GetSizeOfManagedObject<T>(T obj, Type[] additionalSerializableTypes)
            where T : class
        {
            long size = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(T), additionalSerializableTypes);
                serializer.Serialize(ms, obj);
                size = ms.Length;
            }
            return size;
        }
    }
}

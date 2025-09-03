using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractsLayer.Common
{
	/// <summary>
	/// класс менеджера объектов отправки данных
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class ObjectManager<T> where T : class
	{
		/// <summary>
		/// метод получения списка названий полей для изменения
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static List<string> GetListOfObjectPropertyNames(T obj)
		{
			var props = typeof(T).GetProperties();
			return props.Select(p => p.Name).ToList();	
		}
	}
}

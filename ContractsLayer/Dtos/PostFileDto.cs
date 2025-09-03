using ContractsLayer.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractsLayer.Dtos
{
	/// <summary>
	/// модель представления связи файла и поста
	/// </summary>
	public class PostFileDto: BaseEntityModel
	{
		/// <summary>
		/// код публикации
		/// </summary>
		public Guid postId { get; set; }
		/// <summary>
		/// код файла
		/// </summary>
		public Guid fileId { get; set; }
	}
}

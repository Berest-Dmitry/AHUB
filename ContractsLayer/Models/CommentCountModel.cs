using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractsLayer.Models
{
	/// <summary>
	/// модель кол-ва ответов на комментарий
	/// </summary>
	public class CommentCountModel
	{
		public Guid parentId {  get; set; }

		public int childrenCount { get; set; }

		public CommentCountModel(Guid parentId, int childrenCount)
		{
			this.parentId = parentId;
			this.childrenCount = childrenCount;
		}
	}
}

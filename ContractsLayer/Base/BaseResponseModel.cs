using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractsLayer.Base
{
	public class BaseResponseModel<T> : BaseModel where T: class 
	{
		public BaseResponseModel() : base() 
		{ }

		public BaseResponseModel(T Entity) : base()
		{
			this.Entity = Entity;
		}

		public BaseResponseModel(Exception ex, T Entity = null) : base(ex)
		{
			this.Entity = Entity;
		}

		public T Entity { get; set; }
	}
}

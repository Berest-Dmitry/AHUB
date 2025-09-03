using ContractsLayer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ContractsLayer.Base
{
	public class BaseModel
	{
		[JsonIgnore]
		public Exception Error { get; set; }

		public string ErrorInfo
		{
			get
			{
				if (Error != null)
				{
					return Error.Message + (Error.InnerException != null ? Error.InnerException.Message : "");
				}
				else
				{
					return "";
				}
			}
		}

		public DefaultEnums.Result Result { get; set; }

		public BaseModel()
		{
			Result = DefaultEnums.Result.ok;
		}

		public BaseModel(Exception ex)
		{
			Result = DefaultEnums.Result.error;
			Error = ex;
		}

	}

	public static class BaseModelUtilities<T>
		where T : BaseModel, new()
	{
		public static T DataFormat(T CurrentData)
		{
			CurrentData.Result = DefaultEnums.Result.ok;
			return CurrentData;
		}

		public static T ErrorFormat(Exception ex)
		{
			return new T { Error = ex, Result = DefaultEnums.Result.error };
		}
	}
}

using ContractsLayer.Base;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.IServices
{
	/// <summary>
	/// интерфейс сервиса связи файлов с постами
	/// </summary>
	public interface IPostFilesService
	{
		Task<BaseResponseModel<List<PostFileDto>>> AttachFilesToPost(List<Guid> fileIds, Guid postId);


		Task<BaseResponseModel<PostFileDto>> DetachFileFromPost(Guid postId, Guid fileId);

		Task<BaseResponseModel<List<Guid>>> GetFilesAttachedToPost(Guid postId);

		Task<BaseModel> RemoveFilesOfPost(Guid postId);
	}
}

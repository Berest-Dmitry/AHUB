using ContractsLayer.Base;
using ContractsLayer.Common;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicesLayer.IServices;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace AHUB_Test.Controllers
{
	/// <summary>
	/// контроллер для работы с файлами
	/// </summary>
	[Route("api/files")]
	[ApiController]
	public class FilesController : ControllerBase
	{
		private readonly IServiceManager _serviceManager;
		private readonly IConfiguration _configuration;

		public FilesController(IServiceManager serviceManager, IConfiguration configuration)
		{
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		/// <summary>
		/// загрузка файла на сервер
		/// </summary>
		/// <param name="fileUpload"></param>
		/// <returns></returns>
		/// <response code="200"> File upload result </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpPost]
		[Route("upload-file")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<FileDataDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> UploadFile([FromForm] FileUploadDto fileUpload)
		{

			var res = await _serviceManager._filesService.UploadFile(fileUpload.formFile);
			return new JsonResult(res);
		}

		/// <summary>
		/// скачивание файла клиентом
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		/// <response code="200"> File download link </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpGet]
		[Route("download-file")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<FileDownloadDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> DownloadFile(string fileName)
		{
			var res = await _serviceManager._filesService.DownloadFile(fileName);
			return File(res?.stream, res?.contentType, fileName);
		}

		/// <summary>
		/// загрузка файла на сторону клиента
		/// </summary>
		/// <param name="fileId"></param>
		/// <returns></returns>
		/// <response code="200"> File download link </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpGet("{fileId}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<FileDownloadDto>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> LoadFileToClient(Guid fileId)
		{
			var res = await _serviceManager._filesService.GetFile(fileId);
			if(res?.Result == DefaultEnums.Result.error)
			{
				return StatusCode(404, res?.ErrorInfo);
			}
			else
			{
				return File(res?.Entity?.stream, res?.Entity?.contentType, res?.Entity?.fileName);
			}
		}

		/// <summary>
		/// удаление файла
		/// </summary>
		/// <param name="fileId"></param>
		/// <returns></returns>
		/// <response code="200"> File deleted </response>
		/// <response code="401"> Request unauthorized </response>
		[HttpDelete("fileId")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[SwaggerResponse((int)HttpStatusCode.OK, "", typeof(BaseResponseModel<BaseModel>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, "unauthorized request")]
		public async Task<IActionResult> DeleteFile(Guid fileId)
		{
			var res =  await _serviceManager._filesService.DeleteFile(fileId);
			return new JsonResult(res);
		}
	}
}

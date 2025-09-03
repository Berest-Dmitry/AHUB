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
	/// интерфейс сервиса работы с жалобами
	/// </summary>
	public interface IAppealService
	{
		Task<BaseResponseModel<AppealDto>> WriteAppeal(AppealCreateDto appeal);

		Task<BaseResponseModel<List<AppealDto>>> GetAllUserAppeals(Guid userId);

		Task<BaseResponseModel<List<AppealDto>>> GetAppealsBySubject(Guid subjectId);

		Task<BaseResponseModel<AppealDto>> TakeAppealToWork(Guid appealId);

		Task<BaseResponseModel<AppealDto>> ConsiderAppeal(Guid appealId);

		Task<BaseResponseModel<AppealDto>> DenyAppeal(Guid appealId);
	}
}

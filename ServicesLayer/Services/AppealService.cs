using ContractsLayer.Base;
using ContractsLayer.Common;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Endpoints;
using DomainLayer.Models;
using RepositoryLayer.IRepositories;
using ServicesLayer.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Services
{
	/// <summary>
	/// сервис работы с жалобами
	/// </summary>
	public class AppealService: IAppealService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly IServiceManager _serviceManager;

		public AppealService(IRepositoryManager repositoryManager, IServiceManager serviceManager)
		{
			_repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
			_serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
		}

		/// <summary>
		/// метод создания пользовательской жалобы
		/// </summary>
		/// <param name="appeal"></param>
		/// <param name="userName"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<AppealDto>> WriteAppeal(AppealCreateDto appeal)
		{
			try
			{
				var currentUser = await _serviceManager._userService.GetCurrentUser(appeal.userId);
				if(currentUser?.Result != DefaultEnums.Result.ok)
				{
					return new BaseResponseModel<AppealDto>(currentUser?.Error);
				}

				var appealEntry = new Appeal
				{
					reason = (short)appeal.reason,
					comment = appeal.comment,
					userId = appeal.userId,
					appealEntityId = appeal.appealEntityId
				};
				var appealProps = ObjectManager<AppealCreateDto>.GetListOfObjectPropertyNames(appeal);
				var saveRes = await _repositoryManager._appealRepository.CreateAppealEntry(appealEntry, appealProps);

				if(saveRes != null)
				{
					return new BaseResponseModel<AppealDto>
					{
						Entity = ObjectMapper.Mapper.Map<AppealDto>(saveRes),
						Result = DefaultEnums.Result.ok
					};
				}
				else
				{
					return new BaseResponseModel<AppealDto>(
						new Exception("Failed to save user's appeal")
						);
				}
			}
			catch(Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<AppealDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод получения списка всех жалоб пользователя
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<AppealDto>>> GetAllUserAppeals(Guid userId)
		{
			try
			{
				var userAppeals = await _repositoryManager._appealRepository.GetAllAppealsByUser(userId);
				if(userAppeals?.Count > 0)
				{
					var mappedEntity = ObjectMapper.Mapper.Map<List<AppealDto>>(userAppeals);
					return new BaseResponseModel<List<AppealDto>>(mappedEntity);
				}
				else
				{
					return new BaseResponseModel<List<AppealDto>>(
						new Exception("User hasn't created any appeals yet")
						);
				}
			}
			catch(Exception ex) {
				return new BaseResponseModel<List<AppealDto>>(ex);
			}
		}

		/// <summary>
		/// метод получения списка всех пользовательских жалоб, касающихся определенной темы
		/// (например, поста или комментария)
		/// </summary>
		/// <param name="subjectId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<AppealDto>>> GetAppealsBySubject(Guid subjectId)
		{
			try
			{
				var appealsBySubject = await _repositoryManager._appealRepository.GetAllAppealsByEntity(subjectId);
				if(appealsBySubject?.Count > 0)
				{
					var mappedEntity = ObjectMapper.Mapper.Map<List<AppealDto>>(appealsBySubject);
					return new BaseResponseModel<List<AppealDto>>(mappedEntity);
				}
				else
				{
					return new BaseResponseModel<List<AppealDto>>(
						new Exception("There are no complaints on current subject")
						);
				}
			}
			catch (Exception ex)
			{
				return new BaseResponseModel<List<AppealDto>>(ex);
			}
		}

		/// <summary>
		/// метод изменения статуса жалобы
		/// </summary>
		/// <param name="appealId">код жалобы</param>
		/// <param name="status">числовой код статуса<see cref="AppealStatus"/></param>
		/// <returns></returns>
		private async Task<BaseResponseModel<AppealDto>> ChangeAppealStatus(Guid appealId, short status)
		{
			try
			{
				var result = await _repositoryManager._appealRepository.SetStatus(appealId, status);
				if (result != null)
				{
					return new BaseResponseModel<AppealDto>
					{
						Entity = ObjectMapper.Mapper.Map<AppealDto>(result),
						Result = DefaultEnums.Result.ok
					};
				}
				else return new BaseResponseModel<AppealDto>(new Exception("Failed to save new appeal status"));
			}
			catch(Exception ex)
			{
				return BaseModelUtilities<BaseResponseModel<AppealDto>>.ErrorFormat(ex);
			}
		}

		/// <summary>
		/// метод принятия жалобы на рассмотрение
		/// </summary>
		/// <param name="appealId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<AppealDto>> TakeAppealToWork(Guid appealId)
		{
			return await ChangeAppealStatus(appealId, 1);
		}

		/// <summary>
		/// метод принятия решения по жалобе
		/// </summary>
		/// <param name="appealId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<AppealDto>> ConsiderAppeal(Guid appealId)
		{
			return await ChangeAppealStatus(appealId, 2);
		}

		/// <summary>
		/// метод отклонения жалобы
		/// </summary>
		/// <param name="appealId"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<AppealDto>> DenyAppeal(Guid appealId)
		{
			return await ChangeAppealStatus(appealId, 3);
		}
	}
}

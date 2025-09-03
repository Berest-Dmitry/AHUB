using ContractsLayer.Base;
using ContractsLayer.Dtos;

namespace ServicesLayer.IServices
{
    /// <summary>
    /// интерфейс сервиса работы с хештегами
    /// </summary>
    public interface IHashTagService
    {
        Task<BaseResponseModel<List<HashTagDto>>> GetAllHashTags(int skip, int take);

        Task<BaseResponseModel<HashTagDto>> CreateHashTag(string hashTag);

        Task<BaseResponseModel<HashTagDto>> RemoveHashTag(Guid hasTagId);

        Task<BaseResponseModel<List<HashTagDto>>> GetHashTagsByContent(string content,int skip, int take);
    }
}

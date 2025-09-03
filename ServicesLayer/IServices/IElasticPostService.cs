using ContractsLayer.Base;
using ContractsLayer.Dtos;
using ContractsLayer.Dtos.Elastic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.IServices
{
	/// <summary>
	/// интерфейс сервиса по работе с сущностями постов в БД Elasticsearch
	/// </summary>
	public interface IElasticPostService
	{

		Task<BaseResponseModel<PostElDto>> CreatePostEntry(PostDto post);

		Task<BaseResponseModel<PostElDto>> UpdatePostEntry(PostDto post);

		Task<BaseResponseModel<PostElDto>> DeletePostEntry(PostDto post);

		Task<BaseResponseModel<BaseModel>> BulkInsertPosts();

		Task<BaseResponseModel<BaseModel>> DeleteAllPosts();

		Task<BaseResponseModel<List<Guid>>> FuzzySearchPosts(string term, string field);
	}
}

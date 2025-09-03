using ContractsLayer.Dtos.Elastic;
using ContractsLayer.Dtos;
using Nest;
using RepositoryLayer.IRepositories;
using ServicesLayer.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContractsLayer.Base;
using Microsoft.Extensions.Configuration;
using ContractsLayer.Common;
using System.Globalization;

namespace ServicesLayer.Services
{
	/// <summary>
	///  сервис по работе с сущностями постов в БД Elasticsearch
	/// </summary>
	public class ElasticPostService: IElasticPostService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly IElasticClient _client;
		private readonly IConfiguration _configuration;

		public ElasticPostService(IElasticClient client, IRepositoryManager repositoryManager, IConfiguration  con)
		{
			_client = client ?? throw new ArgumentNullException(nameof(client));
			_repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
			_configuration = con ?? throw new ArgumentNullException(nameof(con));
		}

		/// <summary>
		/// метод сохранения поста в эластике
		/// </summary>
		/// <param name="post"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<PostElDto>> CreatePostEntry(PostDto post)
		{
			try
			{
				var elasticDto = ObjectMapper.Mapper.Map<PostElDto>(post);
				var indexResult = await _client.IndexDocumentAsync(elasticDto);
				if (!indexResult.IsValid)
				{
					return new BaseResponseModel<PostElDto>(
						new Exception(indexResult.DebugInformation)
						);
				}
				return new BaseResponseModel<PostElDto>(elasticDto);
			}
			catch(Exception ex)
			{
				return new BaseResponseModel<PostElDto>(ex);
			}
		}

		/// <summary>
		///  метод обновления поста в эластике
		/// </summary>
		/// <param name="post"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<PostElDto>> UpdatePostEntry(PostDto post)
		{
			try
			{
				var elasticDto = ObjectMapper.Mapper.Map<PostElDto>(post);
				var indexRes = await _client.UpdateAsync<PostElDto>(elasticDto.id, e =>
				e.Index(_configuration["ELKConfiguration:index"])
				.Doc(elasticDto));

				if(!indexRes.IsValid)
				{
					return new BaseResponseModel<PostElDto>(
						new Exception(indexRes.DebugInformation)
						);
				}

				return new BaseResponseModel<PostElDto>(elasticDto);
			}
			catch(Exception ex)
			{
				return new BaseResponseModel<PostElDto>(ex);
			}
		}

		/// <summary>
		///  метод удаления поста в эластике
		/// </summary>
		/// <param name="post"></param>
		/// <returns></returns>
		public async Task<BaseResponseModel<PostElDto>> DeletePostEntry(PostDto post)
		{
			try
			{
				var elasticDto = ObjectMapper.Mapper.Map<PostElDto>(post);

				var indexRes = await _client.DeleteAsync<PostElDto>(elasticDto);

				if (!indexRes.IsValid)
				{
					return new BaseResponseModel<PostElDto>(
						new Exception(indexRes.DebugInformation)
						);
				}

				return new BaseResponseModel<PostElDto>(elasticDto);
			}
			catch (Exception ex)
			{
				return new BaseResponseModel<PostElDto>(ex);
			}
		}

		/// <summary>
		/// метод множественной вставки всех постов (публикаций) в БД Elastic
		/// </summary>
		/// <returns></returns>
		public async Task<BaseResponseModel<BaseModel>> BulkInsertPosts()
		{
			try
			{
				var postsList = await _repositoryManager._postsRepository.GetAllPosts(x => x.deletedAt == null);
				var mappedEntity = ObjectMapper.Mapper.Map<List<PostElDto>>(postsList);
				if(mappedEntity?.Count > 0)
				{
					var additionalErrorInfo = "";
					var resp = await _client.IndexManyAsync(mappedEntity);
					if (resp.Errors)
					{
						foreach(var itemWithError in resp.ItemsWithErrors)
						{
							additionalErrorInfo += $"Failed to index document {itemWithError.Id} due to system error: {itemWithError?.Error?.ToString()} \n";
						}

						var messageResponse = new AdditionalMessageModel
						{
							AdditionalErrorMessage = "Request completed, but the following error(s) occurred during task completion: \n" + additionalErrorInfo,
							Result = DefaultEnums.Result.error
						};
						return new BaseResponseModel<BaseModel>(messageResponse);
					}
					

					return new BaseResponseModel<BaseModel>();
				}
				else
				{
					throw new Exception("No posts have been created yet");
				}
			}
			catch(Exception ex )
			{
				return new BaseResponseModel<BaseModel>(ex);
			}
		}

		/// <summary>
		/// метод множественного удаления всех документов (постов) из индекса Elastic
		/// </summary>
		/// <returns></returns>
		public async Task<BaseResponseModel<BaseModel>> DeleteAllPosts()
		{
			try
			{
				var delResponse = await _client.DeleteByQueryAsync<PostElDto>(
					del => del.Query(q => q.QueryString(qs => qs.Query("*"))));

				if (!delResponse.IsValid)
				{
					throw new Exception(delResponse.DebugInformation);
				}
				return new BaseResponseModel<BaseModel>();
			}
			catch(Exception ex )
			{
				return new BaseResponseModel<BaseModel>(ex);
			}
		}

		/// <summary>
		/// метод поиска постов в Elastic
		/// </summary>
		/// <param name="term">поисковый ввод пользователя</param>
		/// <param name="field">поле, по которому осуществляется поиск</param>
		/// <returns></returns>
		public async Task<BaseResponseModel<List<Guid>>> FuzzySearchPosts(string term, string field)
		{
			try
			{
				var searchResponse = await _client.SearchAsync<PostElDto>(s =>
						s.Query(q => BuildFuzzyQueryContainer(q, term, field))
					);

				if (!searchResponse.IsValid)
				{
					throw new Exception(searchResponse.DebugInformation);
				}

				if(searchResponse?.Documents?.Count == 0)
				{
					throw new Exception($"No results found using term: {term}");
				}

				var results = searchResponse?.Documents?
					.Select(d => d.id).ToList();

				return new BaseResponseModel<List<Guid>>(results);
			}
			catch(Exception ex)
			{
				return new BaseResponseModel<List<Guid>>(ex);
			}
		}

		/// <summary>
		/// метод построения fuzzy search запроса к индексу постов в Elastic
		/// </summary>
		/// <param name="queryContainerDescriptor">описание запроса</param>
		/// <param name="term">поисковый ввод пользователя</param>
		/// <param name="field">поле, по которому осуществляется поиск</param>
		/// <returns></returns>
		private QueryContainer BuildFuzzyQueryContainer(QueryContainerDescriptor<PostElDto> queryContainerDescriptor, string term, string field)
		{
			return queryContainerDescriptor
				.Fuzzy(c =>
					c.Name("named_query")
					.Boost(double.Parse(_configuration["ELKConfiguration:Fuzzy:boost"], CultureInfo.InvariantCulture))
					.Fuzziness(Fuzziness.AutoLength(3, 5))
					.Field(field)
					.Value(term)
					.Transpositions()
					.PrefixLength(Convert.ToInt16( _configuration["ELKConfiguration:Fuzzy:prefix_length"]))
					.MaxExpansions(Convert.ToInt16(_configuration["ELKConfiguration:Fuzzy:max_expansions"]))
					.Rewrite(MultiTermQueryRewrite.ConstantScore)
				);
		}
	}
}

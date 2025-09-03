using ContractsLayer.Dtos.Elastic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;


namespace AHUB_Test.Extensions;

/// <summary>
/// класс расширения для использования Elasticsearch
/// </summary>
public static class ElasticSearchExtension
{
	/// <summary>
	/// метод настройки подключения к объекту Elastic
	/// </summary>
	/// <param name="services"></param>
	/// <param name="configuration"></param>
	public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
	{
		string url = OperatingSystem.IsWindows() ? configuration["ELKConfiguration:UrlLocal"] : configuration["ELKConfiguration:UrlDokcer"],
			defaultIndex = configuration["ELKConfiguration:index"];

		var secrets = (configuration["ElasticLogin"], configuration["ElasticPwd"], configuration["ElasticCertificate"]);

		var settings = new ConnectionSettings(new Uri(url))
			.BasicAuthentication(secrets.Item1, secrets.Item2)
			.PrettyJson()
			.CertificateFingerprint(secrets.Item3)
			.DefaultIndex(defaultIndex);

		settings.EnableApiVersioningHeader();
		AddDefaultMappings(settings);

		var client = new ElasticClient(settings);
		services.AddSingleton<IElasticClient>(client);
		CreateElasticIndex(client, defaultIndex);
	}

	/// <summary>
	/// метод добавления сопоставления
	/// между моделью в системе и моделью в Elastic
	/// </summary>
	/// <param name="connectionSettings"></param>
	private static void AddDefaultMappings(ConnectionSettings connectionSettings)
	{
		connectionSettings.DefaultMappingFor<PostElDto>(m => m);
	}

	/// <summary>
	/// метод создания индекса таблицы в Elastic
	/// </summary>
	/// <param name="elasticClient"></param>
	/// <param name="indexName"></param>
	private static void CreateElasticIndex(IElasticClient elasticClient, string indexName)
	{
		var createIndexResponse = elasticClient.Indices
			.Create(indexName, index => index
				.Settings(s => s.Analysis(a => a.Analyzers(aa => aa.Standard("standard_russian", sa => sa.StopWords("_russian_")))))
				.Map<PostElDto>(p => p.AutoMap()));
	}
}

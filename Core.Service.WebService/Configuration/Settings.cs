namespace Core.Service.WebService.Configuration {
	public sealed class Settings {
		public RepositoryMode RepositoryMode        { get; set; }
		public string         MongoConnectionString { get; set; }
		public string         MongoDatabaseName     { get; set; }
	}
}
using Core.Common.Config;

namespace Core.Service.Repository.Config {
	public interface IConfigRepository<TConfig> : IRepository<TConfig> where TConfig : IConfig { }
}
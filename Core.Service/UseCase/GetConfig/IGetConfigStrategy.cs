using System.Threading.Tasks;
using Core.Common.Config;
using Core.Service.Model;

namespace Core.Service.UseCase.GetConfig {
	public interface IGetConfigStrategy<TConfig> where TConfig : IConfig {
		Task<TConfig> GetUserConfig(UserId userId);
	}
}
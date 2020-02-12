using Core.Common.Config;
using Core.Service.Model;

namespace Core.Service.UseCase.GetConfig {
	public interface IGetConfigStrategy<out TConfig> where TConfig : IConfig {
		TConfig GetUserConfig(UserId userId);
	}
}
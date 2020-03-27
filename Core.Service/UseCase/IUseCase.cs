using System.Threading.Tasks;

namespace Core.Service.UseCase {
	public interface IUseCase<in TRequest, TResponse> {
		Task<TResponse> Handle(TRequest request);
	}
}
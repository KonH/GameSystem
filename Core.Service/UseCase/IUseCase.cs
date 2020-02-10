namespace Core.Service.UseCase {
	public interface IUseCase<in TRequest, out TResponse> {
		TResponse Handle(TRequest request);
	}
}
namespace Core.Common.Utils {
	public interface ILoggerFactory {
		ILogger<T> Create<T>();
	}
}
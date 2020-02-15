namespace Core.Client {
	public interface IWebRequestHandler {
		ServiceResponse Post(string url, string body);
	}
}
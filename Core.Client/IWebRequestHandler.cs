namespace Core.Client {
	public interface IWebRequestHandler {
		ServiceResponse<string> Post(string url, string body);
	}
}
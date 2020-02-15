namespace Core.Client {
	public interface IWebRequestHandler {
		string Post(string url, string body);
	}
}
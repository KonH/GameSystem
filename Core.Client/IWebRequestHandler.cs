using System.Threading.Tasks;

namespace Core.Client {
	public interface IWebRequestHandler {
		Task<ServiceResponse> Post(string url, string body);
	}
}
using System.Threading.Tasks;

namespace Core.Client.Web {
	public interface IWebRequestHandler {
		Task<ServiceResponse> Get(string url);
		Task<ServiceResponse> Post(string url, string body);
	}
}
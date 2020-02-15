namespace Clicker.WebService {
	public sealed class WebResponse {
		public object Data { get; set; }

		public WebResponse() { }

		public WebResponse(object data) {
			Data = data;
		}
	}
}
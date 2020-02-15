namespace Core.Common.Utils {
	public sealed class WebResponse<T> {
		public T Data { get; set; }

		public WebResponse() { }

		public WebResponse(T data) {
			Data = data;
		}
	}
}
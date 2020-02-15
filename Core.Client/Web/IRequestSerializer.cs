namespace Core.Client.Web {
	public interface IRequestSerializer {
		string Serialize<T>(T instance);
		T Deserialize<T>(string content);
	}
}
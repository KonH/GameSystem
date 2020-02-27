namespace Core.Client.UnityClient.Settings {
	public interface ISettings {
		ClientMode Mode       { get; }
		string     ConfigPath { get; }
	}
}
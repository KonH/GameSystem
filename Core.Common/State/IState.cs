namespace Core.Common.State {
	public interface IState {
		StateVersion Version { get; set; }
	}
}
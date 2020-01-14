using System;

namespace ResourceMonitor.Writer {
	sealed class CompositeWriter : IWriter, IDisposable {
		readonly IWriter[] _writers;

		public CompositeWriter(params IWriter[] writers) {
			_writers = writers;
		}

		public void Write(SystemInfo info) {
			foreach ( var writer in _writers ) {
				writer.Write(info);
			}
		}

		public void Dispose() {
			foreach ( var writer in _writers ) {
				var disposeWriter = writer as IDisposable;
				disposeWriter?.Dispose();
			}
		}
	}
}
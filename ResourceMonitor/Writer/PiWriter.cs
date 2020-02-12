using System;
using System.Drawing;
using rpi_ws281x;

namespace ResourceMonitor.Writer {
	sealed class PiWriter : IWriter, IDisposable {
		readonly int _width;
		readonly int _height;

		readonly Controller _controller;
		readonly WS281x     _device;

		public PiWriter() {
			_width  = 8;
			_height = 8;
			var settings = Settings.CreateDefaultSettings();
			var ledCount = _width * _height;
			_controller = settings.AddController(ledCount, Pin.Gpio18, StripType.WS2812_STRIP, brightness: 125);
			_device     = new WS281x(settings);
		}

		public void Write(SystemInfo info) {
			_device.Reset();
			RenderNormalizedValue(0, 4, info.CpuUsage, Color.Red);
			RenderNormalizedValue(4, 4, info.MemoryUsage, Color.Green);
			_device.Render();
		}

		void RenderNormalizedValue(int startRow, int rows, double value, Color color) {
			var count = Math.Max(_height * rows * value, 1);
			for ( var i = 0; i < count; i++ ) {
				_controller.SetLED(startRow * _width + i, color);
			}
		}

		public void Dispose() {
			_device.Reset();
			_device.Dispose();
		}
	}
}
using Core.Common.Utils;
using NUnit.Framework;

namespace Core.Common.Tests.Utils {
	public sealed class LoggerFactoryTest {
		[Test]
		public void IsLoggerCreated() {
			var factory = new LoggerFactory(typeof(ConsoleLogger<>));

			Assert.NotNull(factory.Create<LoggerFactoryTest>());
		}
	}
}
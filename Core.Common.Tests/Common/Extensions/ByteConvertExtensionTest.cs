using Core.Common.Extension;
using NUnit.Framework;

namespace Core.Common.Tests.Common.Extensions {
	public sealed class ByteConvertExtensionTest {
		[Test]
		public void IsKbCorrect() => Assert.AreEqual(1024, 1.Kb());
		[Test]
		public void IsMbCorrect() => Assert.AreEqual(1024 * 1024, 1.Mb());
		[Test]
		public void IsGbCorrect() => Assert.AreEqual(1024 * 1024 * 1024, 1.Gb());

		[Test]
		public void IsFromKbCorrect() => Assert.AreEqual(1, 1024.GetKbCount());
		[Test]
		public void IsFromMbCorrect() => Assert.AreEqual(1, (1024 * 1024).GetMbCount());
		[Test]
		public void IsFromGbCorrect() => Assert.AreEqual(1, (1024 * 1024 * 1024).GetGbCount());
	}
}
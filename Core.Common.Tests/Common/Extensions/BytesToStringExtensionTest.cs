using Core.Common.Extension;
using NUnit.Framework;

namespace Core.Common.Tests.Common.Extensions {
	public sealed class BytesToStringExtensionTest {
		[Test]
		public void IsByteCorrect() {
			const long b10 = 10;
			Assert.AreEqual("10 B", b10.ToSizeString());
			Assert.AreEqual("10 B", b10.ToSizeString(true));
		}

		[Test]
		public void IsKbCorrect() {
			const long kb1 = 1024;
			Assert.AreEqual("1 KB", kb1.ToSizeString());
			Assert.AreEqual("1 KB", kb1.ToSizeString(true));

			const long moreThan1Kb = kb1 + 10;
			Assert.AreEqual("1 KB", moreThan1Kb.ToSizeString());
			Assert.AreEqual("1 KB 10 B", moreThan1Kb.ToSizeString(true));
		}

		[Test]
		public void IsMbCorrect() {
			const long mb1 = 1024 * 1024;
			Assert.AreEqual("1 MB", mb1.ToSizeString());
			Assert.AreEqual("1 MB", mb1.ToSizeString(true));

			const long moreThan1Kb = mb1 + 20 * 1024 + 10;
			Assert.AreEqual("1 MB", moreThan1Kb.ToSizeString());
			Assert.AreEqual("1 MB 20 KB 10 B", moreThan1Kb.ToSizeString(true));
		}

		[Test]
		public void IsGbCorrect() {
			const long gb1 = 1024 * 1024 * 1024;
			Assert.AreEqual("1 GB", gb1.ToSizeString());
			Assert.AreEqual("1 GB", gb1.ToSizeString(true));

			const long moreThan1Gb = gb1 + 30 * 1024 * 1024 + 20 * 1024 + 10;
			Assert.AreEqual("1 GB", moreThan1Gb.ToSizeString());
			Assert.AreEqual("1 GB 30 MB 20 KB 10 B", moreThan1Gb.ToSizeString(true));
		}
	}
}
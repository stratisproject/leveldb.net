using LevelDB;

using NUnit.Framework;

namespace DotNetFrameworkTests
{
    [TestFixture]
    public class NativeLibraryVersionTests
    {
        [Test]
        public void Native_library_version()
        {
            Assert.AreEqual(1, LevelDBInterop.leveldb_major_version());
            Assert.AreEqual(23, LevelDBInterop.leveldb_minor_version());
        }
    }
}

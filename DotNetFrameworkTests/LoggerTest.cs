using LevelDB;

using NUnit.Framework;

namespace DotNetFrameworkTests
{
    [TestFixture]
    public class LoggerTest
    {
        [Test]
        public void LoggerTest_Stringable()
        {
            int i = 1;
            byte[] a = { 1, 2, 3 };

            var s = string.Format("{0} {1}", (Stringable)i, (Stringable)a);
        }
    }
}

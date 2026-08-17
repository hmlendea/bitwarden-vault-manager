using System.IO;
using System.Text;

using BitwardenVaultManager.DataAccess;

using NUnit.Framework;

namespace BitwardenVaultManager.UnitTests
{
    [TestFixture]
    public sealed class BitwardenVaultFileHandlerTests
    {
        private BitwardenVaultFileHandler subject;

        [SetUp]
        public void SetUp()
            => subject = new();

        [Test]
        public void GivenAValidVaultFile_WhenLoading_ThenTheVaultIsDeserialised()
        {
            string filePath = Path.GetTempFileName();

            try
            {
                File.WriteAllText(
                    filePath,
                    "{\"encrypted\":false,\"folders\":[],\"items\":[]}",
                    Encoding.UTF8);

                DataAccess.DataObjects.BitwardenVaultEntity vault = subject.Load(filePath);

                Assert.That(vault.Encrypted, Is.False);
                Assert.That(vault.Folders, Is.Empty);
                Assert.That(vault.Items, Is.Empty);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void GivenANullJsonDocument_WhenLoading_ThenAnInvalidDataExceptionIsThrown()
        {
            string filePath = Path.GetTempFileName();

            try
            {
                File.WriteAllText(filePath, "null", Encoding.UTF8);

                TestDelegate action = () => subject.Load(filePath);

                Assert.That(action, Throws.TypeOf<InvalidDataException>().With.Message.Contain(filePath));
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void GivenAnInvalidJsonDocument_WhenLoading_ThenAJsonExceptionIsThrown()
        {
            string filePath = Path.GetTempFileName();

            try
            {
                File.WriteAllText(filePath, "{ invalid json }", Encoding.UTF8);

                TestDelegate action = () => subject.Load(filePath);

                Assert.That(action, Throws.TypeOf<System.Text.Json.JsonException>());
            }
            finally
            {
                File.Delete(filePath);
            }
        }
    }
}
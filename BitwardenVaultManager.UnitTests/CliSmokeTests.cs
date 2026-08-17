using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using BitwardenVaultManager.Menus;
using BitwardenVaultManager.Service;

using Moq;

using NUnit.Framework;

namespace BitwardenVaultManager.UnitTests
{
    [TestFixture]
    public sealed class CliSmokeTests
    {
        private TextReader standardInput;
        private TextWriter standardOutput;
        private Mock<IVaultManager> vaultManagerMock;

        [SetUp]
        public void SetUp()
        {
            standardInput = Console.In;
            standardOutput = Console.Out;
            vaultManagerMock = new();
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetIn(standardInput);
            Console.SetOut(standardOutput);
        }

        [Test]
        public void GivenAStartHook_WhenInvokingProgramMain_ThenTheVaultPathIsStoredAndTheHookIsCalled()
        {
            Type programType = typeof(MainMenu).Assembly.GetType("BitwardenVaultManager.Program")!;
            PropertyInfo startMenuActionProperty = programType.GetProperty("StartMenuAction", BindingFlags.Static | BindingFlags.NonPublic)!;
            PropertyInfo vaultFilePathProperty = programType.GetProperty("VaultFilePath", BindingFlags.Static | BindingFlags.Public)!;
            MethodInfo mainMethod = programType.GetMethod("Main", BindingFlags.Static | BindingFlags.Public)!;
            bool startHookWasCalled = false;
            Action originalAction = (Action)startMenuActionProperty.GetValue(null)!;

            try
            {
                startMenuActionProperty.SetValue(null, (Action)(() => startHookWasCalled = true));

                mainMethod.Invoke(null, [new[] { "vault", ".json" }]);

                Assert.That(startHookWasCalled);
                Assert.That((string)vaultFilePathProperty.GetValue(null)!, Is.EqualTo("vault.json"));
            }
            finally
            {
                startMenuActionProperty.SetValue(null, originalAction);
            }
        }

        [Test]
        public void GivenTotpUrls_WhenInvokingTheMenuCommand_ThenTheUrlsAreWrittenToTheConsole()
        {
            StringWriter outputWriter = new();
            MainMenu menu = new(vaultManagerMock.Object);
            MethodInfo getTotpUrlsMethod = typeof(MainMenu).GetMethod("GetTotpUrls", BindingFlags.Instance | BindingFlags.NonPublic)!;

            vaultManagerMock
                .Setup(vaultManager => vaultManager.GetTotpUrls())
                .Returns(new List<string> { "first-url", "second-url" });
            Console.SetOut(outputWriter);

            getTotpUrlsMethod.Invoke(menu, []);

            Assert.That(outputWriter.ToString(), Does.Contain("first-url"));
            Assert.That(outputWriter.ToString(), Does.Contain("second-url"));
        }
    }
}
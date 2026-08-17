using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BitwardenVaultManager.DataAccess.DataObjects;
using BitwardenVaultManager.Service.Models;

using NUnit.Framework;

namespace BitwardenVaultManager.UnitTests
{
    [TestFixture]
    public sealed class BitwardenMappingsTests
    {
        [Test]
        public void GivenABitwardenFieldEntity_WhenMappingToAServiceModel_ThenThePropertiesAreCopied()
        {
            BitwardenFieldEntity fieldEntity = new()
            {
                Name = "Name",
                Value = "Value"
            };

            BitwardenField mappedField = (BitwardenField)InvokeMappingMethod("BitwardenFieldMappings", "ToServiceModel", fieldEntity);

            Assert.That(mappedField.Name, Is.EqualTo("Name"));
            Assert.That(mappedField.Value, Is.EqualTo("Value"));
        }

        [Test]
        public void GivenABitwardenFieldModel_WhenMappingToADataObject_ThenThePropertiesAreCopied()
        {
            BitwardenField field = new()
            {
                Name = "Name",
                Value = "Value"
            };

            BitwardenFieldEntity mappedField = (BitwardenFieldEntity)InvokeMappingMethod("BitwardenFieldMappings", "ToDataObject", field);

            Assert.That(mappedField.Name, Is.EqualTo("Name"));
            Assert.That(mappedField.Value, Is.EqualTo("Value"));
        }

        [Test]
        public void GivenBitwardenFieldCollections_WhenMapping_ThenEachItemIsMapped()
        {
            List<BitwardenFieldEntity> fieldEntities =
            [
                new() { Name = "One", Value = "1" },
                new() { Name = "Two", Value = "2" }
            ];

            IEnumerable<BitwardenField> mappedFields = ((IEnumerable<BitwardenField>)InvokeMappingMethod("BitwardenFieldMappings", "ToServiceModels", fieldEntities)).ToList();
            IEnumerable<BitwardenFieldEntity> remappedFields = ((IEnumerable<BitwardenFieldEntity>)InvokeMappingMethod("BitwardenFieldMappings", "ToDataObjects", mappedFields)).ToList();

            Assert.That(mappedFields.Select(field => field.Name), Is.EqualTo(new[] { "One", "Two" }));
            Assert.That(remappedFields.Select(field => field.Value), Is.EqualTo(new[] { "1", "2" }));
        }

        [Test]
        public void GivenABitwardenFolderEntity_WhenMappingToAServiceModel_ThenThePropertiesAreCopied()
        {
            Guid folderId = Guid.NewGuid();
            BitwardenFolderEntity folderEntity = new()
            {
                Id = folderId.ToString(),
                Name = "Personal"
            };

            BitwardenFolder mappedFolder = (BitwardenFolder)InvokeMappingMethod("BitwardenFolderMappings", "ToServiceModel", folderEntity);

            Assert.That(mappedFolder.Id, Is.EqualTo(folderId));
            Assert.That(mappedFolder.Name, Is.EqualTo("Personal"));
        }

        [Test]
        public void GivenABitwardenLoginEntity_WhenMappingToAServiceModel_ThenThePropertiesAreCopied()
        {
            BitwardenLoginEntity loginEntity = new()
            {
                Username = "user@example.com",
                Password = "Password123!",
                TOTP = "AB CD"
            };

            BitwardenLogin mappedLogin = (BitwardenLogin)InvokeMappingMethod("BitwardenLoginMappings", "ToServiceModel", loginEntity);

            Assert.That(mappedLogin.Username, Is.EqualTo("user@example.com"));
            Assert.That(mappedLogin.Password, Is.EqualTo("Password123!"));
            Assert.That(mappedLogin.TOTP, Is.EqualTo("ABCD"));
        }

        [Test]
        public void GivenBitwardenLoginCollections_WhenMapping_ThenEachItemIsMapped()
        {
            List<BitwardenLoginEntity> loginEntities =
            [
                new() { Username = "user-one", Password = "PasswordOne", TOTP = "AB CD" },
                new() { Username = "user-two", Password = "PasswordTwo", TOTP = "EF GH" }
            ];

            IEnumerable<BitwardenLogin> mappedLogins = ((IEnumerable<BitwardenLogin>)InvokeMappingMethod("BitwardenLoginMappings", "ToServiceModels", loginEntities)).ToList();
            IEnumerable<BitwardenLoginEntity> remappedLogins = ((IEnumerable<BitwardenLoginEntity>)InvokeMappingMethod("BitwardenLoginMappings", "ToDataObjects", mappedLogins)).ToList();

            Assert.That(mappedLogins.Select(login => login.TOTP), Is.EqualTo(new[] { "ABCD", "EFGH" }));
            Assert.That(remappedLogins.Select(login => login.Username), Is.EqualTo(new[] { "user-one", "user-two" }));
        }

        [Test]
        public void GivenABitwardenItemEntityWithAllOptionalData_WhenMappingToAServiceModel_ThenThePropertiesAreCopied()
        {
            Guid itemId = Guid.NewGuid();
            Guid folderId = Guid.NewGuid();
            BitwardenItemEntity itemEntity = new()
            {
                Id = itemId.ToString(),
                Name = "Example",
                Type = (int)BitwardenItemType.Login,
                FolderId = folderId.ToString(),
                Favourite = true,
                Notes = "Notes",
                Login = new()
                {
                    Username = "user@example.com",
                    Password = "Password123!",
                    TOTP = "AB CD"
                },
                Fields =
                [
                    new() { Name = "Username", Value = "field-user" }
                ]
            };

            BitwardenItem mappedItem = (BitwardenItem)InvokeMappingMethod("BitwardenItemMappings", "ToServiceModel", itemEntity);

            Assert.That(mappedItem.Id, Is.EqualTo(itemId));
            Assert.That(mappedItem.FolderId, Is.EqualTo(folderId));
            Assert.That(mappedItem.IsFavourite, Is.True);
            Assert.That(mappedItem.Notes, Is.EqualTo("Notes"));
            Assert.That(mappedItem.Login, Is.Not.Null);
            Assert.That(mappedItem.Fields!.Single().Value, Is.EqualTo("field-user"));
        }

        [Test]
        public void GivenABitwardenItemEntityWithoutOptionalData_WhenMappingToAServiceModel_ThenTheOptionalPropertiesRemainUnset()
        {
            Guid itemId = Guid.NewGuid();
            BitwardenItemEntity itemEntity = new()
            {
                Id = itemId.ToString(),
                Name = "Example",
                Type = (int)BitwardenItemType.CreditCard,
                FolderId = "",
                Favourite = false,
                Notes = null!,
                Login = null!,
                Fields = null!
            };

            BitwardenItem mappedItem = (BitwardenItem)InvokeMappingMethod("BitwardenItemMappings", "ToServiceModel", itemEntity);

            Assert.That(mappedItem.Id, Is.EqualTo(itemId));
            Assert.That(mappedItem.FolderId, Is.EqualTo(Guid.Empty));
            Assert.That(mappedItem.Login, Is.Null);
            Assert.That(mappedItem.Fields, Is.Null);
        }

        [Test]
        public void GivenABitwardenItemModel_WhenMappingToADataObject_ThenThePropertiesAreCopied()
        {
            Guid itemId = Guid.NewGuid();
            Guid folderId = Guid.NewGuid();
            BitwardenItem item = new()
            {
                Id = itemId,
                Name = "Example",
                Type = BitwardenItemType.Login,
                FolderId = folderId,
                IsFavourite = true,
                Notes = "Notes",
                Login = new()
                {
                    Username = "user@example.com",
                    Password = "Password123!",
                    TOTP = "AB CD"
                },
                Fields =
                [
                    new() { Name = "Name", Value = "Value" }
                ]
            };

            BitwardenItemEntity mappedItem = (BitwardenItemEntity)InvokeMappingMethod("BitwardenItemMappings", "ToDataObject", item);

            Assert.That(mappedItem.Id, Is.EqualTo(itemId.ToString()));
            Assert.That(mappedItem.FolderId, Is.EqualTo(folderId.ToString()));
            Assert.That(mappedItem.Favourite, Is.True);
            Assert.That(mappedItem.Login!.TOTP, Is.EqualTo("ABCD"));
            Assert.That(mappedItem.Fields!.Single().Name, Is.EqualTo("Name"));
        }

        [Test]
        public void GivenABitwardenVaultEntity_WhenMappingToAServiceModel_ThenNestedCollectionsAreMapped()
        {
            Guid folderId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();
            BitwardenVaultEntity vaultEntity = new()
            {
                Encrypted = true,
                Folders =
                [
                    new() { Id = folderId.ToString(), Name = "Personal" }
                ],
                Items =
                [
                    new()
                    {
                        Id = itemId.ToString(),
                        Name = "Entry",
                        Type = (int)BitwardenItemType.Login,
                        FolderId = folderId.ToString(),
                        Favourite = false,
                        Notes = "Note",
                        Login = new() { Username = "user@example.com", Password = "Password123!", TOTP = "ABC" },
                        Fields = []
                    }
                ]
            };

            BitwardenVault mappedVault = (BitwardenVault)InvokeMappingMethod("BitwardenVaultMappings", "ToServiceModel", vaultEntity);

            Assert.That(mappedVault.IsEncrypted, Is.True);
            Assert.That(mappedVault.Folders.Single().Id, Is.EqualTo(folderId));
            Assert.That(mappedVault.Items.Single().Id, Is.EqualTo(itemId));
        }

        [Test]
        public void GivenABitwardenVaultModel_WhenMappingToADataObject_ThenNestedCollectionsAreMapped()
        {
            Guid folderId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();
            BitwardenVault vault = new()
            {
                IsEncrypted = false,
                Folders =
                [
                    new() { Id = folderId, Name = "Personal" }
                ],
                Items =
                [
                    new()
                    {
                        Id = itemId,
                        Name = "Entry",
                        Type = BitwardenItemType.Login,
                        FolderId = folderId,
                        IsFavourite = true,
                        Notes = "Note",
                        Login = new() { Username = "user@example.com", Password = "Password123!", TOTP = "ABC" },
                        Fields = []
                    }
                ]
            };

            BitwardenVaultEntity mappedVault = (BitwardenVaultEntity)InvokeMappingMethod("BitwardenVaultMappings", "ToDataObject", vault);

            Assert.That(mappedVault.Encrypted, Is.False);
            Assert.That(mappedVault.Folders.Single().Id, Is.EqualTo(folderId.ToString()));
            Assert.That(mappedVault.Items.Single().Id, Is.EqualTo(itemId.ToString()));
        }

        [Test]
        public void GivenBitwardenVaultCollections_WhenMapping_ThenEachItemIsMapped()
        {
            Guid firstFolderId = Guid.NewGuid();
            Guid firstItemId = Guid.NewGuid();
            Guid secondFolderId = Guid.NewGuid();
            Guid secondItemId = Guid.NewGuid();
            List<BitwardenVaultEntity> vaultEntities =
            [
                new()
                {
                    Encrypted = false,
                    Folders = [new() { Id = firstFolderId.ToString(), Name = "Personal" }],
                    Items = [new() { Id = firstItemId.ToString(), Name = "Entry One", Type = (int)BitwardenItemType.Login, FolderId = firstFolderId.ToString(), Favourite = false, Notes = "Note", Login = new() { Username = "user-one", Password = "PasswordOne", TOTP = "ABC" }, Fields = [] }]
                },
                new()
                {
                    Encrypted = true,
                    Folders = [new() { Id = secondFolderId.ToString(), Name = "Work" }],
                    Items = [new() { Id = secondItemId.ToString(), Name = "Entry Two", Type = (int)BitwardenItemType.Login, FolderId = secondFolderId.ToString(), Favourite = true, Notes = "Note", Login = new() { Username = "user-two", Password = "PasswordTwo", TOTP = "DEF" }, Fields = [] }]
                }
            ];

            IEnumerable<BitwardenVault> mappedVaults = ((IEnumerable<BitwardenVault>)InvokeMappingMethod("BitwardenVaultMappings", "ToServiceModels", vaultEntities)).ToList();
            IEnumerable<BitwardenVaultEntity> remappedVaults = ((IEnumerable<BitwardenVaultEntity>)InvokeMappingMethod("BitwardenVaultMappings", "ToDataObjects", mappedVaults)).ToList();

            Assert.That(mappedVaults.Select(vault => vault.Folders.Single().Name), Is.EqualTo(new[] { "Personal", "Work" }));
            Assert.That(remappedVaults.Select(vault => vault.Items.Single().Name), Is.EqualTo(new[] { "Entry One", "Entry Two" }));
        }

        private static object InvokeMappingMethod(string mappingClassName, string methodName, object argument)
        {
            Type mappingType = typeof(BitwardenField).Assembly.GetType($"BitwardenVaultManager.Service.Mapping.{mappingClassName}")!;
            MethodInfo method = mappingType
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Single(candidateMethod =>
                    candidateMethod.Name.Equals(methodName, StringComparison.InvariantCulture) &&
                    candidateMethod.GetParameters().Length.Equals(1) &&
                    candidateMethod.GetParameters()[0].ParameterType.IsAssignableFrom(argument.GetType()));

            return method.Invoke(null, [argument])!;
        }
    }
}
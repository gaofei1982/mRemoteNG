﻿using System;
using mRemoteNG.Config.Serializers;
using mRemoteNG.Config.Serializers.Csv;
using mRemoteNG.Connection;
using mRemoteNG.Container;
using mRemoteNG.Credential;
using mRemoteNG.Security;
using mRemoteNG.Tree;
using NSubstitute;
using NUnit.Framework;

namespace mRemoteNGTests.Config.Serializers.ConnectionSerializers.Csv
{
	public class CsvConnectionsSerializerMremotengFormatTests
    {
        private ICredentialRepositoryList _credentialRepositoryList;
        private const string ConnectionName = "myconnection";
        private const string Username = "myuser";
        private const string Domain = "mydomain";
        private const string Password = "mypass123";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var credRecord = Substitute.For<ICredentialRecord>();
            credRecord.Username.Returns(Username);
            credRecord.Domain.Returns(Domain);
            credRecord.Password.Returns(Password.ConvertToSecureString());
            _credentialRepositoryList = Substitute.For<ICredentialRepositoryList>();
            _credentialRepositoryList.GetCredentialRecord(new Guid()).ReturnsForAnyArgs(credRecord);
        }

        [TestCase(Username)]
        [TestCase(Domain)]
        [TestCase(Password)]
        [TestCase("InheritColors")]
        public void CreatesCsv(string valueThatShouldExist)
        {
            var serializer = new CsvConnectionsSerializerMremotengFormat(new SaveFilter(), _credentialRepositoryList);
            var connectionInfo = BuildConnectionInfo();
            var csv = serializer.Serialize(connectionInfo);
            Assert.That(csv, Does.Match(valueThatShouldExist));
        }

        [TestCase(Username)]
        [TestCase(Domain)]
        [TestCase(Password)]
        [TestCase("InheritColors")]
        public void SerializerRespectsSaveFilterSettings(string valueThatShouldntExist)
        {
            var saveFilter = new SaveFilter(true);
            var serializer = new CsvConnectionsSerializerMremotengFormat(saveFilter, _credentialRepositoryList);
            var connectionInfo = BuildConnectionInfo();
            var csv = serializer.Serialize(connectionInfo);
            Assert.That(csv, Does.Not.Match(valueThatShouldntExist));
        }

        [Test]
        public void CanSerializeEmptyConnectionInfo()
        {
            var serializer = new CsvConnectionsSerializerMremotengFormat(new SaveFilter(), _credentialRepositoryList);
            var connectionInfo = new ConnectionInfo();
            var csv = serializer.Serialize(connectionInfo);
            Assert.That(csv, Is.Not.Empty);
        }

        [Test]
        public void CantPassNullToConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new CsvConnectionsSerializerMremotengFormat(null, _credentialRepositoryList));
        }

        [Test]
        public void CantPassNullToSerializeConnectionInfo()
        {
            var serializer = new CsvConnectionsSerializerMremotengFormat(new SaveFilter(), _credentialRepositoryList);
            Assert.Throws<ArgumentNullException>(() => serializer.Serialize((ConnectionInfo)null));
        }

        [Test]
        public void CantPassNullToSerializeConnectionTreeModel()
        {
            var serializer = new CsvConnectionsSerializerMremotengFormat(new SaveFilter(), _credentialRepositoryList);
            Assert.Throws<ArgumentNullException>(() => serializer.Serialize((ConnectionTreeModel)null));
        }

        [Test]
        public void FoldersAreSerialized()
        {
            var serializer = new CsvConnectionsSerializerMremotengFormat(new SaveFilter(), _credentialRepositoryList);
            var container = BuildContainer();
            var csv = serializer.Serialize(container);
            Assert.That(csv, Does.Match(container.Name));
            Assert.That(csv, Does.Match(container.Username));
            Assert.That(csv, Does.Match(container.Domain));
            Assert.That(csv, Does.Match(container.Password));
        }

        private ConnectionInfo BuildConnectionInfo()
        {
            return new ConnectionInfo
            {
                Name = ConnectionName,
				Username = Username,
				Domain = Domain,
				Password = Password,
                Inheritance = {Colors = true}
            };
        }

        private ContainerInfo BuildContainer()
        {
            return new ContainerInfo
            {
                Name = "ThisIsAContainer",
                Username = "BlahBlah1",
                Domain = "aklkskkksh8",
                Password = "qwerasl;kdjf87"
            };
        }
    }
}
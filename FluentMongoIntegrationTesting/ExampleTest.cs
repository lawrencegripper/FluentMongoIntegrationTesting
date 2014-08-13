using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Embedded;
using MongoDB.Driver;

namespace FluentMongoIntegrationTesting
{
    [TestClass]
    public class ExampleTestClass
    {
        private static EmbeddedMongoDbServer mongoEmbedded;
        private static MongoClient mongoClient;
        private static MongoServer mongoServer;

        [ClassInitialize]
        public static void StartMongoEmbedded(TestContext cont)
        {
            mongoEmbedded = new EmbeddedMongoDbServer();
            mongoClient = mongoEmbedded.Client;
            mongoServer = mongoClient.GetServer();
             
        }

        [ClassCleanup]
        public static void ShutdownMongo()
        {
            mongoEmbedded.Dispose();
        }

        [TestInitialize]
        public void CleanMongo()
        {
            var databases = mongoServer.GetDatabaseNames();
            foreach (var databaseName in databases)
            {
                mongoServer.DropDatabase(databaseName);
            }
        }

        [TestMethod]
        public void ExampleTest()
        {
            //arrange
            var databaseName = "testDatabase";
            var collectionName = typeof(ExampleType).Name;
            var db = mongoServer.GetDatabase(databaseName);
            var collection = db.GetCollection<ExampleType>(collectionName);
            
            //act
            var item = new ExampleType() { MyProperty = 1 };
            collection.Insert(item);

            //assert
            mongoServer.AssertDatabaseExists(databaseName)
                .AssertCollectionExists<ExampleType>()
                .AssertCollectionItemCount(1)
                .AssertCollectionHasItemWithProperty<ExampleType>(item, x => x.MyProperty == 1);
        }

        public class ExampleType
        {
            public ObjectId Id { get; set; }
            public int MyProperty { get; set; }
        }

    }
}

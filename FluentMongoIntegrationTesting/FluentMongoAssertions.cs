using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FluentMongoIntegrationTesting
{
    public static class FluentMongoAssertions
    {
        public static MongoDatabase AssertDatabaseExists(this MongoServer mongoServer, string databaseName)
        {
            var container = mongoServer.DatabaseExists(databaseName);
            if (!container)
            {
                Assert.Fail(string.Format("Database doesn't exist '{0}'", databaseName));
            }

            Trace.WriteLine(string.Format("Database exists '{0}'", databaseName));

            return mongoServer.GetDatabase(databaseName);
        }

        public static MongoCollection<T> AssertCollectionExists<T>(this MongoDatabase mongoDatabase)
        {
            var collectionName = typeof(T).Name;
            if (!mongoDatabase.CollectionExists(collectionName))
            {
                Assert.Fail(string.Format("Collection doesn't exist '{0}'", collectionName));
            }

            Trace.WriteLine(string.Format("Collection exists '{0}'", collectionName));

            return mongoDatabase.GetCollection<T>(collectionName);
        }

        public static MongoCollection<T> AssertCollectionItemCount<T>(this MongoCollection<T> mongoCollection, int count)
        {
            if (mongoCollection.Count() != count)
            {
                Assert.Fail(string.Format("Collection doesn't have correct count expected '{0}' actual '{1}'", count, mongoCollection.Count()));
            }

            Trace.WriteLine(string.Format("Collection had expected items count '{0}'", mongoCollection.Count()));

            return mongoCollection;
        }


        public static MongoCollection<T> AssertCollectionHasItemWithProperty<T>(this MongoCollection<T> mongoCollection, T item, Expression<Func<T, bool>> itemSelector, int expectNumberOfItems = 1)
        {
            var queryable = mongoCollection.AsQueryable<T>();
            var output = queryable.Where(itemSelector);

            if (output.Count() != expectNumberOfItems)
            {
                Assert.Fail(string.Format("Collection doesn't have '{0}' items '{1}' in collection '{2}' when selected with '{3}'",expectNumberOfItems, typeof(T).Name, mongoCollection.Name, itemSelector.Body.ToString()));

            }

            Trace.WriteLine(string.Format("Collection has '{0}' items '{1}' in collection '{2}' selected with '{3}'",expectNumberOfItems, typeof(T).Name, mongoCollection.Name, itemSelector.Body.ToString()));

            return mongoCollection;
        }
    }
}

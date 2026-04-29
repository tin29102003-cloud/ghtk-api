using MongoDB.Driver;

namespace Ghtk.Api.Bootstraping
{
    public static class MongoDbClientExtentions
    {
        public static void AddMongoDbClient(this IServiceCollection services, IConfiguration configuration)
        {

            var mongoClient = new MongoClient(configuration.GetConnectionString("MongoDbConnection"));
            //var databaseName = configuration["MongoDb:DatabaseName"] ?? "ghtk";
            services.AddSingleton(mongoClient);
            //services.AddSingleton(sp => mongoClient.GetDatabase(databaseName));
        }
    }
}

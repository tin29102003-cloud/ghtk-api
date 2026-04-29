using Ghtk.Repository.Abstractions;
using Ghtk.Repository.Abstractions.Entities;
using MongoDB.Driver;

namespace Ghtk.Repository.MongoDb
{
    public class MongoDbOrderRepository : IOrderRepository
    {
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<OrderEntity> _orderCollection;
        public MongoDbOrderRepository(MongoClient mongoClient)
        {
            _mongoClient = mongoClient;
            _database = mongoClient.GetDatabase("ghtk");
            _orderCollection = _database.GetCollection<OrderEntity>("order");//dòng này là lấy collection "order" từ database "ghtk" và gán cho biến _orderCollection, sau đó có thể sử dụng biến này để thực hiện các thao tác CRUD trên collection "order" trong MongoDB.
        }

        public  async Task<bool> CancelOrderAsync(string id, string partnerId, int status)
        {
            var filter = Builders<OrderEntity>.Filter.Eq(o => o.TrackingId, id) & Builders<OrderEntity>.Filter.Eq(o => o.PartnerId, partnerId); 
            var update = Builders<OrderEntity>.Update.Set(o => o.Status, status); // Cập nhật trạng thái thành "đã hủy"
            var r =  await  _orderCollection.UpdateOneAsync(filter, update);
            return r.ModifiedCount > 0;
        }

        public async Task CreateOrderAsync(OrderEntity orderEntity)
        {
           await this._orderCollection.InsertOneAsync(orderEntity);
        }

        public Task<OrderEntity> GetOrderByIdAsync(string id, string partnerId)
        {
            var filter = Builders<OrderEntity>.Filter.Eq(o => o.TrackingId, id) & Builders<OrderEntity>.Filter.Eq(o => o.PartnerId, partnerId);
            return _orderCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}

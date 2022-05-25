using Catalog.Entities;
using MongoDB.Driver;

namespace Catalog.Repositories;

public class MongoDbItemsRepository : IItemsRepository
{
    private const string DATABASE_NAME = "catalog";
    private const string COLLECTION_NAME = "items";

    private readonly IMongoCollection<Item> _items;
    private readonly FilterDefinitionBuilder<Item> _filterBuilder = Builders<Item>.Filter;

    public MongoDbItemsRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase(DATABASE_NAME);
        _items = database.GetCollection<Item>(COLLECTION_NAME);
    }

    public async Task CreateItemAsync(Item item)
    {
        await _items.InsertOneAsync(item);
    }

    public async Task DeleteItemAsync(Guid id)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        await _items.DeleteOneAsync(filter);
    }

    public async Task<Item?> GetItemAsync(Guid id)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        return await _items.Find(filter).SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<Item>> GetItemsAsync()
    {
        return await _items.Find(x => true).ToListAsync();
    }

    public async Task UpdateItemAsync(Item item)
    {
        var filter = _filterBuilder.Eq(x => x.Id, item.Id);
        await _items.ReplaceOneAsync(filter, item);
    }
}

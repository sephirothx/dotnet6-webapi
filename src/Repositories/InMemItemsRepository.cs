using Catalog.Entities;

namespace Catalog.Repositories;

public class InMemItemsRepository : IItemsRepository
{
    private readonly List<Item> _items = new List<Item>()
    {
        new Item()
        {
            Id = Guid.NewGuid(),
            Name = "Potion",
            Price = 9,
            CreatedDate = new DateTimeOffset(2018, 10, 1, 0, 0, 0, TimeSpan.Zero)
        },
        new Item()
        {
            Id = Guid.NewGuid(),
            Name = "Sword",
            Price = 20,
            CreatedDate = new DateTimeOffset(2018, 10, 1, 0, 0, 0, TimeSpan.Zero)
        },
        new Item()
        {
            Id = Guid.NewGuid(),
            Name = "Shield",
            Price = 18,
            CreatedDate = new DateTimeOffset(2018, 10, 1, 0, 0, 0, TimeSpan.Zero)
        }
    };

    public async Task<IEnumerable<Item>> GetItemsAsync()
    {
        return await Task.FromResult(_items);
    }

    public async Task<Item?> GetItemAsync(Guid id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        return await Task.FromResult(item);
    }

    public Task CreateItemAsync(Item item)
    {
        _items.Add(item);

        return Task.CompletedTask;
    }

    public Task UpdateItemAsync(Item item)
    {
        var index = _items.FindIndex(x => x.Id == item.Id);
        _items[index] = item;

        return Task.CompletedTask;
    }

    public Task DeleteItemAsync(Guid id)
    {
        var index = _items.FindIndex(x => x.Id == id);
        _items.RemoveAt(index);

        return Task.CompletedTask;
    }
}

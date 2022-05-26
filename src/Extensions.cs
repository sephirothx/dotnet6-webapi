namespace Catalog;

using Catalog.Dtos;
using Catalog.Entities;

public static class Extensions
{
    public static ItemDto AsDto(this Item item)
        => new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
}

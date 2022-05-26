namespace Catalog;

using Catalog.Dtos;
using Catalog.Entities;

public static class Extensions
{
    public static ItemDto AsDto(this Item item)
        => new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price,
            CreatedDate = item.CreatedDate
        };
}

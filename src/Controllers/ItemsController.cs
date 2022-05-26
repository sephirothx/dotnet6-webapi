namespace Catalog.Controllers;

using System;
using System.Collections.Generic;
using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("items")]
public class ItemsController : Controller
{
    private readonly IItemsRepository _itemsRepository;

    public ItemsController(IItemsRepository itemsRepository)
    {
        _itemsRepository = itemsRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<ItemDto>> GetItemsAsync()
    {
        var items = (await _itemsRepository.GetItemsAsync()).Select(x => x.AsDto());

        return items;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
    {
        var item = await _itemsRepository.GetItemAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        return item.AsDto();
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
    {
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = itemDto.Name,
            Description = itemDto.Description,
            Price = itemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await _itemsRepository.CreateItemAsync(item);

        return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
    {
        var item = await _itemsRepository.GetItemAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        item.Name = itemDto.Name;
        item.Description = itemDto.Description;
        item.Price = itemDto.Price;

        await _itemsRepository.UpdateItemAsync(item);

        return NoContent();
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> DeleteItemAsync(Guid id)
    {
        var item = await _itemsRepository.GetItemAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        await _itemsRepository.DeleteItemAsync(id);

        return NoContent();
    }
}

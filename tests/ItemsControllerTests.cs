using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Controllers;
using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Catalog.Tests;

public class ItemsControllerTests
{
    private readonly Mock<IItemsRepository> _mockItemRepository = new();
    private readonly Random _random = new();

    [Fact]
    public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
    {
        // Arrange
        _mockItemRepository.Setup(x => x.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(null as Item);

        var controller = new ItemsController(_mockItemRepository.Object);

        // Act
        var result = await controller.GetItemAsync(Guid.NewGuid());

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
    {
        // Arrange
        var expectedItem = CreateItem();

        _mockItemRepository.Setup(x => x.GetItemAsync(expectedItem.Id))
            .ReturnsAsync(expectedItem);

        var controller = new ItemsController(_mockItemRepository.Object);

        // Act
        var result = await controller.GetItemAsync(expectedItem.Id);

        // Assert
        result.Value.Should().BeEquivalentTo(
            expectedItem,
            options => options.ComparingByMembers<Item>());
    }

    [Fact]
    public async Task GetItemsAsync_WithExistingItems_ReturnsAllItems()
    {
        // Arrange
        var expectedItems = new[] { CreateItem(), CreateItem(), CreateItem() };

        _mockItemRepository.Setup(x => x.GetItemsAsync())
            .ReturnsAsync(expectedItems);

        var controller = new ItemsController(_mockItemRepository.Object);

        // Act
        var result = await controller.GetItemsAsync();

        // Assert
        result.Should().BeEquivalentTo(
            expectedItems,
            options => options.ComparingByMembers<Item>());
    }

    [Fact]
    public async Task GetItemsAsync_WithMatchingItems_ReturnsMatchingItems()
    {
        // Arrange
        var items = new[]
        {
            new Item { Id = Guid.NewGuid(), Name = "Pass 1" },
            new Item { Id = Guid.NewGuid(), Name = "Fail 1" },
            new Item { Id = Guid.NewGuid(), Name = "Pass 2" }
        };

        var nameToMatch = "Pass";

        _mockItemRepository.Setup(x => x.GetItemsAsync())
            .ReturnsAsync(items);

        var controller = new ItemsController(_mockItemRepository.Object);

        // Act
        IEnumerable<ItemDto> result = await controller.GetItemsAsync(nameToMatch);

        // Assert
        result.Should().OnlyContain(
            item => item.Name!.Contains(nameToMatch, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task CreateItemAsync_WithValidItem_ReturnsCreatedAtRouteResult()
    {
        // Arrange
        var expectedItem = new CreateItemDto(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            _random.Next(1, 1000));
        var controller = new ItemsController(_mockItemRepository.Object);

        // Act
        var result = await controller.CreateItemAsync(expectedItem);

        // Assert
        var createdItem = (result.Result as CreatedAtActionResult)!.Value as ItemDto;
        createdItem.Should().BeEquivalentTo(
            expectedItem,
            options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers());
    }

    [Fact]
    public async Task UpdateItemAsync_WithValidItem_ReturnsNoContent()
    {
        // Arrange
        var expectedItem = CreateItem();
        _mockItemRepository.Setup(x => x.GetItemAsync(expectedItem.Id))
            .ReturnsAsync(expectedItem);

        var itemDto = new UpdateItemDto(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            _random.Next(1, 1000));

        var controller = new ItemsController(_mockItemRepository.Object);

        // Act
        var result = await controller.UpdateItemAsync(expectedItem.Id, itemDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateItemAsync_WithUnexistingItem_ReturnsNotFound()
    {
        // Arrange
        _mockItemRepository.Setup(x => x.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(null as Item);

        var itemDto = new UpdateItemDto(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            _random.Next(1, 1000));

        var controller = new ItemsController(_mockItemRepository.Object);

        // Act
        var result = await controller.UpdateItemAsync(Guid.NewGuid(), itemDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteItemAsync_WithUnexistingItem_ReturnsNotFound()
    {
        // Arrange
        _mockItemRepository.Setup(x => x.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(null as Item);

        var controller = new ItemsController(_mockItemRepository.Object);

        // Act
        var result = await controller.DeleteItemAsync(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
    {
        // Arrange
        var expectedItem = CreateItem();
        _mockItemRepository.Setup(x => x.GetItemAsync(expectedItem.Id))
            .ReturnsAsync(expectedItem);

        var controller = new ItemsController(_mockItemRepository.Object);

        // Act
        var result = await controller.DeleteItemAsync(expectedItem.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    private Item CreateItem()
    {
        return new Item
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Price = _random.Next(1, 1000),
            CreatedDate = DateTimeOffset.UtcNow
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using lab12v5;

namespace lab12v5.Tests;

public class CustomerRepositoryTests
{
    [Fact]
    public void Add_ShouldAddCustomer_WhenItemIsValid()
    {
        // Arrange
        var repository = new InMemoryRepository<Customer>();
        var customer = new Customer { Id = 1, FirstName = "Тарас", LastName = "Шевченко", Address = "м. Канів" };

        // Act
        repository.Add(customer);
        var result = repository.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Тарас", result.FirstName);
        Assert.Single(repository.GetAll());
    }

    [Fact]
    public void GetById_ShouldReturnCustomer_WhenIdExists()
    {
        // Arrange
        var repository = new InMemoryRepository<Customer>();
        var customer = new Customer { Id = 1, FirstName = "Леся", LastName = "Українка", Address = "м. Луцьк" };
        repository.Add(customer);

        // Act
        var result = repository.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Леся", result.FirstName);
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenIdDoesNotExist()
    {
        // Arrange
        var repository = new InMemoryRepository<Customer>();

        // Act
        var result = repository.GetById(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ShouldReturnAllCustomers()
    {
        // Arrange
        var repository = new InMemoryRepository<Customer>();
        repository.Add(new Customer { Id = 1, FirstName = "A", LastName = "A", Address = "Addr1" });
        repository.Add(new Customer { Id = 2, FirstName = "B", LastName = "B", Address = "Addr2" });

        // Act
        var items = repository.GetAll().ToList();

        // Assert
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public void Update_ShouldModifyExistingCustomer()
    {
        // Arrange
        var repository = new InMemoryRepository<Customer>();
        repository.Add(new Customer { Id = 1, FirstName = "Олег", LastName = "Бойко", Address = "Стара адреса" });

        var updatedCustomer = new Customer { Id = 1, FirstName = "Олег", LastName = "Бойко", Address = "Нова адреса" };

        // Act
        repository.Update(updatedCustomer);
        var result = repository.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Нова адреса", result.Address);
    }

    [Fact]
    public void Update_ShouldThrowKeyNotFoundException_WhenCustomerDoesNotExist()
    {
        // Arrange
        var repository = new InMemoryRepository<Customer>();
        var nonExistentCustomer = new Customer { Id = 99, FirstName = "Ghost", LastName = "User", Address = "None" };

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => repository.Update(nonExistentCustomer));
    }

    [Fact]
    public void Delete_ShouldRemoveCustomer_WhenIdExists()
    {
        // Arrange
        var repository = new InMemoryRepository<Customer>();
        repository.Add(new Customer { Id = 1, FirstName = "Марія", LastName = "Франко", Address = "м. Львів" });

        // Act
        repository.Delete(1);

        // Assert
        Assert.Empty(repository.GetAll());
        Assert.Null(repository.GetById(1));
    }

    [Fact]
    public void Delete_ShouldThrowKeyNotFoundException_WhenIdDoesNotExist()
    {
        // Arrange
        var repository = new InMemoryRepository<Customer>();

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => repository.Delete(999));
    }
}
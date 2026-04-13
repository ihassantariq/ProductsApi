using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductsApi.Models;

namespace ProductsApi.Tests.Integration;

public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetTokenAsync()
    {
        var loginRequest = new LoginRequest { Username = "admin", Password = "admin" };
        var response = await _client.PostAsJsonAsync("/api/Auth/Token", loginRequest);
        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return loginResponse!.Token;
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk_WithoutAuth()
    {
        // Act
        var response = await _client.GetAsync("/api/Health/Check");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsUnauthorized_WithoutToken()
    {
        // Act
        var response = await _client.GetAsync("/api/Products/GetAllProducts");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ReturnsUnauthorized_WithoutToken()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            Description = "Desc",
            Price = 10.00m,
            Colour = "Red"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Products/CreateProduct", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Auth_ReturnsToken_WithValidCredentials()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = "admin", Password = "admin" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/Token", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResponse);
        Assert.False(string.IsNullOrEmpty(loginResponse.Token));
    }

    [Fact]
    public async Task Auth_ReturnsUnauthorized_WithInvalidCredentials()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = "wrong", Password = "wrong" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/Token", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsSeededProducts_WithToken()
    {
        // Arrange
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/Products/GetAllProducts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        Assert.NotNull(products);
        Assert.True(products.Count >= 6); // 6 seeded products
    }

    [Fact]
    public async Task GetAllProducts_FiltersByColour_WithToken()
    {
        // Arrange
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/Products/GetAllProducts?colour=Red");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        Assert.NotNull(products);
        Assert.All(products, p => Assert.Equal("Red", p.Colour));
    }

    [Fact]
    public async Task CreateProduct_CreatesAndReturnsProduct_WithToken()
    {
        // Arrange
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateProductRequest
        {
            Name = "Integration Test Product",
            Description = "Created during integration test",
            Price = 99.99m,
            Colour = "Purple"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Products/CreateProduct", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(product);
        Assert.Equal("Integration Test Product", product.Name);
        Assert.Equal("Purple", product.Colour);
        Assert.Equal(99.99m, product.Price);
    }

    [Fact]
    public async Task CreateProduct_ReturnsBadRequest_WithInvalidData()
    {
        // Arrange
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateProductRequest
        {
            Name = "",
            Description = "Missing name",
            Price = -5.00m,
            Colour = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Products/CreateProduct", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task FullFlow_CreateAndRetrieveProduct()
    {
        // Arrange
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create a product
        var createRequest = new CreateProductRequest
        {
            Name = "Flow Test Product",
            Description = "Full flow test",
            Price = 55.00m,
            Colour = "Orange"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/Products/CreateProduct", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        // Retrieve all products and verify new product exists
        var getResponse = await _client.GetAsync("/api/Products/GetAllProducts?colour=Orange");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var products = await getResponse.Content.ReadFromJsonAsync<List<ProductResponse>>();
        Assert.NotNull(products);
        Assert.Contains(products, p => p.Name == "Flow Test Product" && p.Colour == "Orange");
    }
}

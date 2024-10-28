using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhraseApi.Core.Entities;
using PhraseApi.Infrastructure.Data;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace PhraseApi.Tests.Integration
{
    public class PhraseApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public PhraseApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the real db context
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<PhrasesDbContext>));
                    
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add in-memory database
                    services.AddDbContext<PhrasesDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestingDb");
                    });
                });
            });
        }

        [Fact]
        public async Task CreatePhrase_ValidData_ReturnsCreatedResponse()
        {
            // Arrange
            var client = _factory.CreateClient();
            var phrase = new Phrase { Text = "Test phrase" };

            // Act
            var response = await client.PostAsJsonAsync("/api/phrases", phrase);
            var createdPhrase = await response.Content.ReadFromJsonAsync<Phrase>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            createdPhrase.Should().NotBeNull();
            createdPhrase!.Text.Should().Be("Test phrase");
            createdPhrase.IsActive.Should().BeTrue();
            createdPhrase.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            createdPhrase.Id.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("", "The Text field is required")]
        [InlineData(null, "The Text field is required")]
        public async Task CreatePhrase_EmptyOrNullText_ReturnsBadRequest(string? text, string expectedError)
        {
            // Arrange
            var client = _factory.CreateClient();
            var phrase = new Phrase { Text = text! };

            // Act
            var response = await client.PostAsJsonAsync("/api/phrases", phrase);
            var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            error.Should().NotBeNull();
            error!.Errors.Should().ContainKey("Text");
            error.Errors["Text"].Should().Contain(expectedError);
        }

        [Fact]
        public async Task CreatePhrase_PersistsToDatabase()
        {
            // Arrange
            var client = _factory.CreateClient();
            var phrase = new Phrase { Text = "Test persistence" };

            // Act
            var response = await client.PostAsJsonAsync("/api/phrases", phrase);
            var createdPhrase = await response.Content.ReadFromJsonAsync<Phrase>();

            // Get a new scope to verify database persistence
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PhrasesDbContext>();
            var savedPhrase = await dbContext.Phrases.FindAsync(createdPhrase!.Id);

            // Assert
            savedPhrase.Should().NotBeNull();
            savedPhrase!.Text.Should().Be("Test persistence");
        }
    }
}
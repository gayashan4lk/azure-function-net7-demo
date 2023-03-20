using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.CosmosRepository;
using azure_function_net7_demo.Models;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text;
using System.Text.Json;
using System.Linq.Expressions;
using azure_function_net7_demo.Api.CustomerApi;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net;

namespace azure_function_net7_demo.test
{
    public class CreateCustomerTests
    {
        private readonly Mock<ILoggerFactory> loggerFactoryMock;
        private readonly Mock<IRepository<Customer>> customerRepositoryMock;

        public CreateCustomerTests()
        {
            loggerFactoryMock = new Mock<ILoggerFactory>();
            customerRepositoryMock = new Mock<IRepository<Customer>>();
        }

        [Fact]
        public async Task Run_ValidRequest_ReturnsOk()
        {
            // Arrange
            var guid = Guid.NewGuid().ToString();
            var existingCutomer = new Customer()
            {
                Id = guid,
                FirstName = "Test",
                LastName = "Test",
                Email = "test@test.com",
                Type = "Customer"
            };
            var requestBody = new { FirstName = "John", Email = "john@example.com" };
            var httpRequestMock = new Mock<HttpRequestData>();
            httpRequestMock.Setup(r => r.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(requestBody))));
            var httpResponseMock = new Mock<HttpResponseData>();

            //customerRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(existingCutomer);
            //customerRepositoryMock.Setup(x => x.CreateAsync()).ReturnsAsync(new Customer());

            var sut = new CreateCustomer(loggerFactoryMock.Object, customerRepositoryMock.Object);

            // Act
            var result = await sut.Run(httpRequestMock.Object);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            httpResponseMock.VerifySet(r => r.StatusCode = HttpStatusCode.OK);

        }
    }
}
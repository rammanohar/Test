using Moq;
using MediatR;
using Microsoft.Extensions.Logging;
using Test.Core;        // Your User model namespace
using Test.Queries;     // Your GetUsersQueryByBatch namespace
using Test.Service;     // Your UserService namespace

namespace Test.UnitTests
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<ILogger<UserService>> _loggerMock;
        private UserService _service;

        [TestInitialize]
        public void Setup()
        {
            // 1. Mock the internal dependencies your service constructor needs
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<UserService>>();

            // 2. Instantiate the service with the mocked dependencies
            _service = new UserService(_mediatorMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetUsersBatchAsync_ShouldSendMediatRQueryAndReturnData()
        {
            // Arrange - Define your input parameters
            int recordsToSkip = 10;
            int batchCount = 5;
            var cancellationToken = CancellationToken.None;

            // Generate the fake/mocked data that you expect to "come up"
            var expectedUsers = new List<User>
            {
                new User(1, 1, "Alice", "alice@test.com", DateTime.UtcNow),
                new User(2, 2, "Bob", "bob@test.com", DateTime.UtcNow)
            }.AsQueryable();

            // Set up the Mock: When MediatR sends the matching query record, return our fake users list
            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<GetUsersQueryByBatch>(q => q.recordsToSkip == recordsToSkip && q.batchCount == batchCount),
                    cancellationToken))
                .ReturnsAsync(expectedUsers);

            // Act - Invoke your service method directly
            var result = await _service.GetUsersBatchAsync(recordsToSkip, batchCount, cancellationToken);
            var resultList = result.ToList();

            // Assert - Verify the data came up correctly
            Assert.IsNotNull(result);
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual("Alice", resultList[0].Name);
            Assert.AreEqual("Bob", resultList[1].Name);

            // Verify MediatR was actually triggered exactly once
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetUsersQueryByBatch>(), cancellationToken), Times.Once);
        }
    }
}

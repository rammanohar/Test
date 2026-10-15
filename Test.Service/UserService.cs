using MediatR;
using Microsoft.Extensions.Logging;
using Test.Commands;
using Test.Core;
using Test.Interface;
using Test.Queries;
namespace Test.Service
{
    public class UserService : IUserService
    {


        private readonly IMediator _mediator;
        private readonly ILogger<UserService> _logger;

        public UserService(IMediator mediator, ILogger<UserService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        public async Task<bool> ExecuteBulkInsertAsync(List<User> users, CancellationToken ct)
        {
            return await _mediator.Send(new BulkInsertUsersCommand(users), ct);
        }
               

        public Task<int> GetAllUsersAsync(CancellationToken ct)
        {
            _logger.LogInformation("Executing GetGetAllUsersAsync method.");

            if (ct.IsCancellationRequested)
            {
                _logger.LogWarning("Operation was cancelled.");
                ct.ThrowIfCancellationRequested();
            }
            
            var count  =  _mediator.Send(new GetAllUsersQuery(), ct);

            return count;
        }

       

        public async Task<User> GetUserByIdAsync(int Id, CancellationToken ct)
        {
            _logger.LogInformation("Executing GetUserByIdAsync method.");
            var user = await _mediator.Send(new GetUserById(Id), ct);

            return user;
        }

        public async Task<IQueryable<User>> GetUsersBatchAsync(int recordsToSkip, int batchCount, CancellationToken ct)
        {
            _logger.LogInformation("Executing GetUsersBatchAsync method.");
            var users = await _mediator.Send(new GetUsersQueryByBatch(recordsToSkip, batchCount), ct);
            return users;
        }
    }


}

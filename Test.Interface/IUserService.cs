using Test.Commands;
using Test.Core;
using Test.Queries;

namespace Test.Interface
{
    public interface IUserService
    {
        Task<bool> ExecuteBulkInsertAsync(List<User> users, CancellationToken ct);

        Task<int> GetAllUsersAsync(CancellationToken ct);
        Task<User> GetUserByIdAsync(int Id, CancellationToken ct);

        Task<IQueryable<User>> GetUsersBatchAsync(int recordsToSkip, int batchCount, CancellationToken ct);
    }
}

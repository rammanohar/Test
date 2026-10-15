using Test.Core;
using MediatR;

namespace Test.Commands
{
    public record BulkInsertUsersCommand(IEnumerable<User> users) : IRequest<bool>;
    
}

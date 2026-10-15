using MediatR;  
using Test.Core;    
namespace Test.Queries
{
    public record GetUserById(int Id) : IRequest<User>;
}
    
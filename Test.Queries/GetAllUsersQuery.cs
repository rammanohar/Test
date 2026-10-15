using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Test.Queries
{
    public record GetAllUsersQuery : IRequest<int>;               

}

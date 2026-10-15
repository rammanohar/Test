using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Test.Core;

namespace Test.Queries
{
    public record GetUsersQueryByBatch(int recordsToSkip,int batchCount) : IRequest<IQueryable<User>>;  
   
}

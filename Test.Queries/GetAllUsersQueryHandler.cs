using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Test.EFEntities;
using Microsoft.EntityFrameworkCore;

namespace Test.Queries
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, int>
    {
        private readonly TestDbContext _context;


        public GetAllUsersQueryHandler(TestDbContext context)
        {
            _context = context;
        }
        public Task<int> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return _context.SourceUsers.CountAsync(cancellationToken);
        }
    }
}

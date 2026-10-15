using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Test.Core;
using Test.EFEntities;

namespace Test.Queries
{
    public class GetUsersQueryByBatchHandler : IRequestHandler<GetUsersQueryByBatch, IQueryable<User>>
    {
        private readonly TestDbContext _context;
        
        public GetUsersQueryByBatchHandler(TestDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<User>> Handle(GetUsersQueryByBatch request, CancellationToken cancellationToken)
        {
            var users = await _context.SourceUsers
                .OrderBy(u => u.Id)
                .Take(request.batchCount)
                .Skip(request.recordsToSkip)
                .Select(u => new User
                (
                    u.Id,
                    u.ExternalId,
                    u.Name,
                    u.Email,
                    u.CreatedAt
                ))
                .ToListAsync(cancellationToken);
            if (users == null) { throw new InvalidOperationException("User not found"); }
            return  users.AsQueryable();
        }
    }
}

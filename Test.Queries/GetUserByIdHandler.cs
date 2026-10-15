using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Test.Core;
using Test.EFEntities;

namespace Test.Queries
{
    public class GetUserByIdHandler : IRequestHandler<GetUserById, User>
    {
        private readonly TestDbContext _context;

        public GetUserByIdHandler(TestDbContext context)
        {
            _context = context;
        }
        public async Task<User> Handle(GetUserById request, CancellationToken cancellationToken)
        {
            var user = await _context.SourceUsers
                .Where(u => u.Id == request.Id)
                .Select(u => new User
                ( 
                    u.Id,
                    u.ExternalId,
                    u.Name,
                    u.Email,
                    u.CreatedAt
                ))
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null) { throw new InvalidOperationException("User not found"); }
            return user;
        }   
    }
}

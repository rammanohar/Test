using Azure.Core;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using Test.Core;
using Test.EFEntities;
namespace Test.Commands
{
    public class BulkInsertUsersCommandHandlers : IRequestHandler<BulkInsertUsersCommand, bool>
    {

        private readonly TestDbContext _context;
        private readonly ILogger<BulkInsertUsersCommandHandlers> _logger;
        public BulkInsertUsersCommandHandlers(TestDbContext context, ILogger<BulkInsertUsersCommandHandlers> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> Handle(BulkInsertUsersCommand request, CancellationToken cancellationToken)
        {
            if (request.users == null || request.users.Any() )
            {
                return true;
            }

            try
            {


                using DataTable dataTable = CreateBulkCopyTable(request.users);

                using var connection = new SqlConnection(_context.Database.GetConnectionString());

                await connection.OpenAsync(cancellationToken);

                using var transaction = connection.BeginTransaction();

                using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction)
                {
                    DestinationTableName = "dbo.SourceUsers",
                    BatchSize = 10000,
                    BulkCopyTimeout = 180
                };

                bulkCopy.ColumnMappings.Add("ExternalId", "ExternalId");
                bulkCopy.ColumnMappings.Add("Name", "Name");
                bulkCopy.ColumnMappings.Add("Email", "Email");
                bulkCopy.ColumnMappings.Add("CreatedAt", "CreatedAt");

                try
                {
                    // 6. Direct memory-to-disk streaming execution
                    await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    _logger.LogInformation("Successfully completed bulk writing operation.");
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Bulk copy operation collapsed. Structural rollback triggered.");
                    return false;
                }
                //// 2. Map your List<User> from the command payload to EF Database Entities
                //var entitiesToInsert = request.users.Select(u => new SourceUser
                //{
                //   //Id = u.Id,
                //    ExternalId = u.ExternalId,
                //    CreatedAt = u.CreatedAt,
                //    Name = u.Name,
                //    Email = u.Email
                //}).ToList();

                //// 3. Add the entire range to the Entity Framework tracking state cache
                //// Note: AddRangeAsync is highly efficient for staging collections in memory
                //await _context.SourceUsers.AddRangeAsync(entitiesToInsert, cancellationToken);

                //// 4. Physical single-transaction commit execution to your MS SQL instance
                //var rowsAffected = await _context.SaveChangesAsync(cancellationToken);

                // Returns true if all staged entries were successfully written
                //return rowsAffected > 0;
            }
            catch (Exception)
            {
                // Fail safely if database constraints, table mismatches, or network errors occur
                return false;
            }
        }

        private static DataTable CreateBulkCopyTable(IEnumerable<User> users)
        {
            var table = new DataTable();
            table.Columns.Add("ExternalId", typeof(int));
            table.Columns.Add("CreatedAt", typeof(DateTime));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Email", typeof(string));

            foreach (var u in users)
            {
                var row = table.NewRow();
                row["ExternalId"] = u.ExternalId;
                row["CreatedAt"] = u.CreatedAt;
                row["Name"] = u.Name ?? string.Empty;
                row["Email"] = u.Email ?? string.Empty;
                table.Rows.Add(row);
            }

            return table;
        }

    }
}

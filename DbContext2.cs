using Microsoft.EntityFrameworkCore;

namespace RazorClassLibrary
{
    public class DbContext2 : DbContext
    {
        public DbContext2(DbContextOptions options) : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //var entries = ChangeTracker.Entries().Where(x => x.Entity is ICreatedModified);

            //foreach (var entry in entries.Where(x => x.State == EntityState.Added))
            //{

            //}

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

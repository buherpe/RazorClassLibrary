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
            var entries = ChangeTracker.Entries();

            //foreach (var entry in entries)
            //{
            //    Console.WriteLine($"{entry.Entity} {entry.State}");
            //}

            //Console.WriteLine($"{ChangeTracker.DebugView.ShortView}");

            return base.SaveChangesAsync(cancellationToken);
        }

        public override void Dispose()
        {
            Console.WriteLine($"DbContext2.Dispose");
            base.Dispose();
        }
    }
}

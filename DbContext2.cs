using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace RazorClassLibrary
{
    public class DbContext2 : DbContext
    {
        public int? CurrentUserId { get; set; }

        public DbContext2(DbContextOptions options) : base(options)
        {
        }

        public void SetCreatedModifiedToEntity(IEntity entity, DateTime dateTime)
        {
            if (entity is not ICreatedModified createdModifiedEntity)
            {
                return;
            }

            Entry(createdModifiedEntity).Property(x => x.CreatedAt).IsModified = false;
            Entry(createdModifiedEntity).Property(x => x.CreatedById).IsModified = false;

            if (entity.Id == 0)
            {
                createdModifiedEntity.CreatedAt = dateTime;
                createdModifiedEntity.CreatedById = CurrentUserId;
            }
            else
            {
                Entry(createdModifiedEntity).Property(x => x.ModifiedAt).IsModified = false;
                Entry(createdModifiedEntity).Property(x => x.ModifiedById).IsModified = false;

                if (Entry(entity).State == EntityState.Modified)
                {
                    createdModifiedEntity.ModifiedAt = dateTime;
                    createdModifiedEntity.ModifiedById = CurrentUserId;
                }
            }
        }

        public void SetCreatedModifiedToEntities()
        {
            var entries = ChangeTracker.Entries();

            var now = DateTime.Now;

            foreach (var entry in entries)
            {
                if (entry.Entity is not IEntity entity)
                {
                    continue;
                }

                SetCreatedModifiedToEntity(entity, now);
            }
        }

        public Task<int> SaveAsync(bool setCreatedModified = true, CancellationToken cancellationToken = default)
        {
            if (setCreatedModified)
            {
                SetCreatedModifiedToEntities();
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

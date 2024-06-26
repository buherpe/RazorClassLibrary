﻿using Microsoft.EntityFrameworkCore;

namespace RazorClassLibrary;

public interface IBaseFactory
{
    Task<IEntity> GetById(int id);

    Task Delete(IEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
}

//public class BaseFactory : IBaseFactory
//{
//    public MyContext Context { get; set; } = new();

//    public Type EntityType { get; set; }

//    public async Task<object> GetById(int id)
//    {
//        return await Context.FindAsync(EntityType, id);
//    }

//    public async Task Delete(object entity, bool saveChanges = true, CancellationToken cancellationToken = default)
//    {
//        Context.Remove(entity);

//        if (saveChanges)
//        {
//            await Context.SaveChangesAsync(cancellationToken);
//        }
//    }
//}

public class BaseFactory<TEntity>(DbContext context) : IBaseFactory
    where TEntity : class, IEntity
    //where TContext : DbContext, new()
{
    public async Task<IEntity> GetById(int id)
    {
        return await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Delete(IEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        context.Remove(entity);

        if (saveChanges)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    //public async Task SaveAsync()
    //{

    //}
}

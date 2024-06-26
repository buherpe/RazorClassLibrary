﻿using Microsoft.EntityFrameworkCore;

namespace RazorClassLibrary;

public interface IView
{
    IQueryable GetData(string filter = null);

    string GetName();

    string GetNames();

    string GetEntityName();

    string GetEntityNames();

    void Qwe(IEntity entity);
}

public interface IEntity
{
    int Id { get; set; }
}

public class BaseView<TEntity> : IView
    where TEntity : class, IEntity, ICreatedModified
{
    public virtual IQueryable GetData(string filter)
    {
        throw new System.NotImplementedException();
    }

    public virtual string GetName()
    {
        throw new System.NotImplementedException();
    }

    public virtual string GetNames()
    {
        throw new System.NotImplementedException();
    }

    public virtual string GetEntityName()
    {
        throw new System.NotImplementedException();
    }

    public virtual string GetEntityNames()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Qwe(IEntity entity)
    {
        throw new System.NotImplementedException();
    }

    public IQueryable<TEntity> BaseInclude(IQueryable<TEntity> queryable)
    {
        return Include(queryable.Include(x => x.CreatedBy).Include(x => x.ModifiedBy));
    }

    public virtual IQueryable<TEntity> Include(IQueryable<TEntity> queryable)
    {
        return queryable;
    }
}

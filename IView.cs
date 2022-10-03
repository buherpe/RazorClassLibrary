using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RazorClassLibrary
{
    public interface IView
    {
        IQueryable GetData(string filter = null);

        string GetName();

        string GetNames();

        string GetEntityName();

        string GetEntityNames();

        void Qwe(IEntity entity);

        IBaseFactory GetNewFactory();
    }

    public interface IEntity
    {
        int Id { get; set; }
    }

    public class BaseView<TEntity, TBaseFactory, TContext> : IView
        where TEntity : class, IEntity
        where TBaseFactory : BaseFactory<TEntity, TContext>, new()
        where TContext : DbContext, new()
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

        public IBaseFactory GetNewFactory()
        {
            return new TBaseFactory();
        }
    }

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

    public class BaseFactory<TEntity, TContext> : IBaseFactory
        where TEntity : class, IEntity
        where TContext : DbContext, new()
    {
        public TContext Context { get; set; } = new();

        //public BaseFactory()
        //{
        //    //EntityType = typeof(TEntity);
        //}

        public async Task<IEntity> GetById(int id)
        {
            return await Context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Delete(IEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            Context.Remove(entity);

            if (saveChanges)
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

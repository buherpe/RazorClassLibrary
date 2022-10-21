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

        //IQueryable<IEntity> Include(IQueryable<IEntity> queryable);
    }

    public interface IEntity
    {
        int Id { get; set; }
    }

    public class BaseView<TEntity> : IView
        where TEntity : class, IEntity
        //where TBaseFactory : BaseFactory<TEntity/*, TContext*/>, new()
        //where TContext : DbContext, new()
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
            throw new System.NotImplementedException();
        }

        public virtual IQueryable<TEntity> Include(IQueryable<TEntity> queryable)
        {
            return queryable;
        }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorClassLibrary
{
    public partial class EditComponent<TEntity, TFactory, TView, TContext>
        where TEntity : class, IEntity, new()
        where TFactory : BaseFactory<TEntity, TContext>, new()
        where TView : IView, new()
        where TContext : DbContext, new()
    {
        [Parameter]
        public int Id { get; set; }

        public TEntity Entity { get; set; } = new();

        public bool Loading { get; set; } = true;

        [Parameter]
        public TView View { get; set; } = new();

        public TContext Context { get; set; } = new();

        // https://stackoverflow.com/q/63955228
        //protected override async Task OnParametersSetAsync()
        protected override async Task OnInitializedAsync()
        {
            Loading = true;

            await Load(Id);

            Loading = false;
        }

        public async Task Save()
        {
            Loading = true;

            //var f = new TFactory();
            Context.Set<TEntity>().Update(Entity);
            
            await Context.SaveChangesAsync();

            NavigationManager.NavigateTo($"/{View.GetEntityNames()}/{Entity.Id}");

            await Load(Entity.Id);

            Loading = false;
        }

        public async Task Load(int id)
        {
            //var f = new TFactory();
            
            var dbSet = Context.Set<TEntity>();

            var queryable = Include(dbSet);

            Entity = await queryable.FirstOrDefaultAsync(x => x.Id == id) ?? new TEntity();
        }

        public virtual IQueryable<TEntity> Include(DbSet<TEntity> dbSet)
        {
            return dbSet;
        }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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

        public TEntity Entity { get; set; }

        public bool Loading { get; set; } = true;

        [Parameter]
        public TView View { get; set; } = new();

        public TContext Context { get; set; } = new();

        public EditContext editContext;

        // https://stackoverflow.com/q/63955228
        // ситуация: Blazored.Modal вызывал ререндер и поэтому
        // OnParametersSetAsync срабатывал много раз, вызывая лишний Load
        // было исправлено в https://github.com/Blazored/Modal/issues/459 (7.1.0)
        //protected override async Task OnInitializedAsync()
        protected override async Task OnParametersSetAsync()
        {
            Loading = true;

            await Load(Id);

            Loading = false;
        }

        public async Task Save()
        {
            if (!editContext.Validate())
            {
                return;
            }

            Loading = true;

            //var f = new TFactory();
            Context.Set<TEntity>().Update(Entity);

            var isNew = Entity.Id == 0;

            await Context.SaveChangesAsync();

            Context = new TContext();

            if (isNew)
            {
                NavigationManager.NavigateTo($"/{View.GetEntityNames()}/{Entity.Id}");
            }

            //await Load(Entity.Id);

            Loading = false;
        }

        public async Task Load(int id)
        {
            if (id == 0)
            {
                Entity = new TEntity();
            }
            else
            {
                var dbSet = Context.Set<TEntity>();

                var queryable = Include(dbSet);

                Entity = await queryable.FirstOrDefaultAsync(x => x.Id == id) ?? new TEntity();
            }

            editContext = new(Entity);
            editContext.SetFieldCssClassProvider(new CustomFieldClassProvider());
        }

        public virtual IQueryable<TEntity> Include(DbSet<TEntity> dbSet)
        {
            return dbSet;
        }
    }
}

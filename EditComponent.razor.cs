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
        where TView : BaseView<TEntity, TFactory, TContext>, new()
        where TContext : DbContext, new()
    {
        [Parameter]
        public int Id { get; set; }

        public TEntity Entity { get; set; }

        public bool Loading { get; set; } = true;

        [Parameter]
        public TView View { get; set; } = new();

        public TContext Context { get; set; } = new();

        public EditContext EditContext;

        // https://stackoverflow.com/q/63955228
        // ситуация: Blazored.Modal вызывал ререндер и поэтому
        // OnParametersSetAsync срабатывал много раз, вызывая лишний Load
        // было исправлено в https://github.com/Blazored/Modal/issues/459 (7.1.0)
        // сейчас не будет работать переход например с таски 5 на 6,
        // потому что OnParametersSetAsync закомменчен
        // иначе он будет срабатывать многократно при смене наименования например
        protected override async Task OnInitializedAsync()
        //protected override async Task OnParametersSetAsync()
        {
            Loading = true;

            await Load(Id);

            Loading = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Console.WriteLine($"OnAfterRenderAsync");
        }

        public async Task Save()
        {
            if (!EditContext.Validate())
            {
                return;
            }

            Loading = true;

            //var f = new TFactory();
            var isNew = Entity.Id == 0;

            if (Entity is ICreatedModified createdModifiedEntity)
            {
                var user = (await AuthState).User;

                if (isNew)
                {
                    createdModifiedEntity.CreatedAt = DateTime.Now;
                    createdModifiedEntity.CreatedBy = int.Parse(user.FindFirst("Id").Value);
                }
                else
                {
                    createdModifiedEntity.ModifiedAt = DateTime.Now;
                    createdModifiedEntity.ModifiedBy = int.Parse(user.FindFirst("Id").Value);
                }
            }

            Context.Set<TEntity>().Update(Entity);

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
            //Console.WriteLine($"Load");

            if (id == 0)
            {
                Entity = new TEntity();
            }
            else
            {
                var queryable = View.Include(Context.Set<TEntity>());

                //await Task.Delay(500);
                Entity = await queryable.FirstOrDefaultAsync(x => x.Id == id) ?? new TEntity();
            }

            EditContext = new(Entity);
            EditContext.SetFieldCssClassProvider(new CustomFieldClassProvider());
        }
    }
}

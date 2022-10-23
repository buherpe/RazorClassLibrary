using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace RazorClassLibrary
{
    public partial class EditComponent<TEntity, TView>
        where TEntity : class, IEntity, new()
        where TView : BaseView<TEntity>, new()
    {
        [Parameter]
        public int Id { get; set; }

        public TEntity Entity { get; set; }

        public bool Loading { get; set; } = true;

        [Parameter]
        public TView View { get; set; } = new();

        public EditContext EditContext;

        // https://stackoverflow.com/q/63955228
        // ситуация: Blazored.Modal вызывал ререндер и поэтому
        // OnParametersSetAsync срабатывал много раз, вызывая лишний Load
        // было исправлено в https://github.com/Blazored/Modal/issues/459 (7.1.0)
        // сейчас не будет работать переход например с таски 5 на 6,
        // потому что OnParametersSetAsync закомменчен
        // иначе он будет срабатывать каждый раз при смене наименования например
        protected override async Task OnInitializedAsync()
        //protected override async Task OnParametersSetAsync()
        {
            //Console.WriteLine($"OnInitializedAsync");

            await Load(Id);
        }

        //protected override void OnInitialized()
        //{
        //    Console.WriteLine($"OnInitialized");
        //}

        protected override void OnAfterRender(bool firstRender)
        {
            //Console.WriteLine($"OnAfterRenderAsync");
        }

        public async Task Save()
        {
            //Console.WriteLine($"Save");

            if (!EditContext.Validate())
            {
                return;
            }

            Loading = true;

            //var f = new TFactory();
            var isNew = Entity.Id == 0;

            //Context.ChangeTracker.Clear();

            if (Entity is ICreatedModified createdModifiedEntity)
            {
                Context.Entry(createdModifiedEntity).Property(x => x.CreatedAt).IsModified = false;
                Context.Entry(createdModifiedEntity).Property(x => x.CreatedBy).IsModified = false;

                var user = (await AuthState).User;

                if (isNew)
                {
                    createdModifiedEntity.CreatedAt = DateTime.Now;
                    createdModifiedEntity.CreatedBy = int.Parse(user.FindFirst("Id").Value);
                }
                else
                {
                    Context.Entry(createdModifiedEntity).Property(x => x.ModifiedAt).IsModified = false;
                    Context.Entry(createdModifiedEntity).Property(x => x.ModifiedBy).IsModified = false;

                    if (Context.Entry(Entity).State == EntityState.Modified)
                    {
                        createdModifiedEntity.ModifiedAt = DateTime.Now;
                        createdModifiedEntity.ModifiedBy = int.Parse(user.FindFirst("Id").Value);
                    }
                }
            }

            //Console.WriteLine($"{Context.Entry(Entity).DebugView.LongView}");

            // https://stackoverflow.com/a/50355281/6414543
            //Context.Set<TEntity>().Update(Entity);
            var existingEntity = await View.Include(Context.Set<TEntity>()).FirstOrDefaultAsync(x => x.Id == Entity.Id);

            if (existingEntity == null)
            {
                // если делать Add, то связанная сущность пытается сохраниться
                // например, при создании задачи если выбирать проект, то он становится Added
                // и при сохранении падает эксепшон что такой проект уже есть
                //Context.Add(Entity);
                Context.Entry(Entity).State = EntityState.Added;
            }
            else
            {
                Context.Entry(existingEntity).CurrentValues.SetValues(Entity);
            }

            await Context.SaveChangesAsync();

            //await Context.Entry(Entity).ReloadAsync();

            //Context = new TContext();
            //Context.Attach(Entity);

            //Entity = await View.Include(Context.Set<TEntity>()).FirstOrDefaultAsync(x => x.Id == Entity.Id);

            if (isNew)
            {
                NavigationManager.NavigateTo($"/{View.GetEntityNames()}/{Entity.Id}");
            }

            //Id = Entity.Id;
            await Load(Entity.Id);

            //Loading = false;
        }

        public async Task Load(int id)
        {
            //Console.WriteLine($"Load");

            Loading = true;

            //todo
            //Context = new();

            if (id == 0)
            {
                Entity = new TEntity();
            }
            else
            {
                //await Task.Delay(500);
                
                var entity = await View.Include(Context.Set<TEntity>()).FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    Entity = new TEntity();
                }
                else
                {
                    Entity = entity;

                    //foreach (var item in Context.Entry(entity).Navigations)
                    //{
                    //    var q = item.EntityEntry;
                    //}
                }
            }

            EditContext = new(Entity);
            EditContext.SetFieldCssClassProvider(new CustomFieldClassProvider());

            Loading = false;
        }
    }
}

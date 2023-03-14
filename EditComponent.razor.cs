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
    public partial class EditComponent<TEntity, TView, TContext> : IDisposable
        where TEntity : class, IEntity, ICreatedModified, new()
        where TView : BaseView<TEntity>, new()
        where TContext : DbContext2
    {
        [Parameter]
        public int Id { get; set; }

        public TEntity Entity { get; set; }

        public bool Loading { get; set; } = true;

        [Parameter]
        public TView View { get; set; } = new();

        public EditContext EditContext;

        private TContext _context;

        private int? PrevId { get; set; }

        // https://stackoverflow.com/q/63955228
        // ситуация: Blazored.Modal вызывал ререндер и поэтому
        // OnParametersSetAsync срабатывал много раз, вызывая лишний Load
        // было исправлено в https://github.com/Blazored/Modal/issues/459 (7.1.0)
        // сейчас не будет работать переход например с таски 5 на 6,
        // потому что OnParametersSetAsync закомменчен
        // иначе он будет срабатывать каждый раз при смене наименования например
        //protected override async Task OnInitializedAsync()
        //{
        //    //Console.WriteLine($"OnInitializedAsync, {Id}, {_id}");

        //    //await Load(Id);
        //}

        protected override async Task OnParametersSetAsync()
        {
            //Console.WriteLine($"OnParametersSetAsync, Id: {Id}, PrevId: {PrevId}");

            // если айди сменился или предыдущего нет (только создали), то загружаем
            if (Id != PrevId || PrevId == null)
            {
                //Console.WriteLine($"OnParametersSetAsync: ");
                await Load(Id);
            }

            PrevId = Id;

            //Console.WriteLine($"OnParametersSetAsync, {Id}, {_id}");
        }

        //protected override void OnInitialized()
        //{
        //    Console.WriteLine($"OnInitialized");
        //}

        //protected override void OnAfterRender(bool firstRender)
        //{
        //    //Console.WriteLine($"OnAfterRenderAsync");
        //}

        [Parameter]
        public EventCallback<FieldChangedEventArgs> OnFieldChanged { get; set; }

        public async Task<bool> Save()
        {
            //Console.WriteLine($"Save");

            if (!EditContext.Validate())
            {
                return false;
            }

            Loading = true;

            //var f = new TFactory();
            var isNew = Entity.Id == 0;

            //Context.ChangeTracker.Clear();

            //if (Entity is ICreatedModified createdModifiedEntity)
            //{
            //    _context.Entry(createdModifiedEntity).Property(x => x.CreatedAt).IsModified = false;
            //    _context.Entry(createdModifiedEntity).Property(x => x.CreatedById).IsModified = false;

            //    var user = (await AuthState).User;

            //    if (isNew)
            //    {
            //        createdModifiedEntity.CreatedAt = DateTime.Now;
            //        createdModifiedEntity.CreatedById = int.Parse(user.FindFirst("Id").Value);
            //    }
            //    else
            //    {
            //        _context.Entry(createdModifiedEntity).Property(x => x.ModifiedAt).IsModified = false;
            //        _context.Entry(createdModifiedEntity).Property(x => x.ModifiedById).IsModified = false;

            //        if (_context.Entry(Entity).State == EntityState.Modified)
            //        {
            //            createdModifiedEntity.ModifiedAt = DateTime.Now;
            //            createdModifiedEntity.ModifiedById = int.Parse(user.FindFirst("Id").Value);
            //        }
            //    }
            //}

            var user = (await AuthState).User;
            _context.CurrentUserId = int.Parse(user.FindFirst("Id").Value);
            //_context.SetCreatedModifiedToEntities();
            _context.SetCreatedModifiedToEntity(Entity, DateTime.Now);

            //Console.WriteLine($"{Context.Entry(Entity).DebugView.LongView}");

            // https://stackoverflow.com/a/50355281/6414543
            //Context.Set<TEntity>().Update(Entity);
            var existingEntity = await View.BaseInclude(_context.Set<TEntity>()).FirstOrDefaultAsync(x => x.Id == Entity.Id);

            if (existingEntity == null)
            {
                // если делать Add, то связанная сущность пытается сохраниться
                // например, при создании задачи если выбирать проект, то он становится Added
                // и при сохранении падает эксепшон что такой проект уже есть
                //Context.Add(Entity);
                _context.Entry(Entity).State = EntityState.Added;
            }
            else
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(Entity);
            }

            await _context.SaveAsync(false);

            EditContext.MarkAsUnmodified();

            ConfirmExternalNavigation = false;

            //await Context.Entry(Entity).ReloadAsync();

            //Context = new TContext();
            //Context.Attach(Entity);

            //Entity = await View.Include(Context.Set<TEntity>()).FirstOrDefaultAsync(x => x.Id == Entity.Id);

            toastService.ShowSuccess("Сохранено");

            //if (isNew)
            //{
            //    //Console.WriteLine($"navigate to /{View.GetEntityNames()}/{Entity.Id}");
            //    NavigationManager.NavigateTo($"/{View.GetEntityNames()}/{Entity.Id}");
            //}

            //Id = Entity.Id;
            //await Load(Entity.Id);

            //Loading = false;

            return true;
        }

        public async Task Load(int id)
        {
            //Console.WriteLine($"Load: id: {id}");

            Loading = true;

            //Context = new();
            // хз нужно ли руками диспозить, пока оставил
            _context?.Dispose();
            _context = contextFactory.CreateDbContext();

            if (id == 0)
            {
                Entity = new TEntity();
            }
            else
            {
                //await Task.Delay(500);
                
                var entity = await View.BaseInclude(_context.Set<TEntity>()).FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    Entity = new TEntity();
                }
                else
                {
                    Entity = entity;

                    // если так не делать, то при перевыборе например проекта в задаче
                    // при сохранении будет эксепшон что такая ентитя уже тречится
                    _context.ChangeTracker.Clear();
                    _context.Entry(Entity).State = EntityState.Unchanged;
                }
            }

            EditContext = new(Entity);
            EditContext.SetFieldCssClassProvider(new CustomFieldClassProvider());
            EditContext.OnFieldChanged += async (sender, e) =>
            {
                await OnFieldChanged.InvokeAsync(e);
                ConfirmExternalNavigation = true;
            };

            Loading = false;
        }

        public async Task SaveAndLoad()
        {
            var isNew = Entity.Id == 0;

            if (!await Save())
            {
                return;
            }

            //Console.WriteLine($"{Entity.Id}");

            if (isNew)
            {
                //Console.WriteLine($"navigate to /{View.GetEntityNames()}/{Entity.Id}");
                NavigationManager.NavigateTo($"/{View.GetEntityNames()}/{Entity.Id}");
            }
            else
            {
                await Load(Entity.Id);
            }
        }

        public void Dispose()
        {
            //Console.WriteLine($"EditComponent.Dispose");
            _context.Dispose();
        }
    }
}

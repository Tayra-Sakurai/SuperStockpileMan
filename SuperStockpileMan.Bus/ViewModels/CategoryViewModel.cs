using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SuperStockpileMan.Bus.Contexts;
using SuperStockpileMan.Bus.Messages;
using SuperStockpileMan.Bus.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.ViewModels
{
    public partial class CategoryViewModel : ObservableValidator
    {
        private readonly IDbContextFactory<SuperStockpileManContext> factory;
        private Category category;

        public CategoryViewModel(IDbContextFactory<SuperStockpileManContext> factory)
        {
            this.factory = factory;
            category = new();
            Children = [];
            Categories = [];
        }

        private async Task LoadCategoriesAsync()
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            EntityEntry<Category> entityEntry = context.Attach(category);
            await entityEntry
                .Reference(e => e.Parent)
                .LoadAsync();
            OnPropertyChanged(nameof(Parent));

            Categories.Clear();

            Categories.Add(null);

            await foreach (
                Category category in
                context
                .Categories
                .OrderBy(e => e.Id)
                .AsAsyncEnumerable())
                Categories.Add(category);
        }

        private async Task LoadChildrenAsync(Category category)
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            Stack<Category> stack = new();
            stack.Push(category);

            while (stack.Count > 0)
            {
                Category current = stack.Pop();
                await context
                    .Entry(current)
                    .Collection(e => e.Children)
                    .LoadAsync();

                foreach (CategoryBase categoryBase in current.Children)
                    if (categoryBase is Category category1)
                        stack.Push(category1);
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            if (category.Children.Count == 0)
                await LoadChildrenAsync(category);

            Children.Clear();

            foreach (CategoryBase categoryBase in category.Children)
                Children.Add(categoryBase);

            await LoadCategoriesAsync();
        }

        public async Task LoadExistingCategoryAsync(Category category)
        {
            this.category = category;

            if (category.Children.Count == 0)
            {
                await LoadChildrenAsync(category);
            }

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Parent));

            await LoadCategoriesAsync();

            RemoveCommand.NotifyCanExecuteChanged();
            SaveCommand.NotifyCanExecuteChanged();
            AddCommand.NotifyCanExecuteChanged();
        }

        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages))]
        public string Name
        {
            get => category.Name;
            set
            {
                if (SetProperty(category.Name, value, category, (m, v) => m.Name = v, true))
                {
                    SaveCommand.NotifyCanExecuteChanged();
                    AddCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public CategoryBase? Parent
        {
            get => category.Parent;
            set => SetProperty(category.Parent, value, category, (m, v) => m.Parent = v, true);
        }

        [ObservableProperty]
        private ObservableCollection<CategoryBase> children;

        [ObservableProperty]
        private ObservableCollection<Category?> categories;

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanRemove))]
        private async Task RemoveAsync()
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            context.Remove(category);

            await context.SaveChangesAsync();

            WeakReferenceMessenger.Default.Send(new CategoryRemovedMessage(category));
        }

        private bool CanRemove()
        {
            using SuperStockpileManContext context = factory.CreateDbContext();

            EntityEntry<Category> entityEntry = context.Attach(category);
            return entityEntry.State == EntityState.Unchanged;
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            context.Update(category);
            await context.SaveChangesAsync();
        }

        private bool CanSave()
        {
            ValidateAllProperties();
            return !HasErrors;
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task AddAsync()
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            context.Add(category);
            await context.SaveChangesAsync();

            WeakReferenceMessenger.Default.Send(new CategoryAddedMessage(category));
        }
    }
}

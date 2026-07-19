using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using SuperStockpileMan.Bus.Contexts;
using SuperStockpileMan.Bus.Messages;
using SuperStockpileMan.Bus.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.ViewModels
{
    public partial class CategoriesViewModel : ObservableObject
    {
        private IDbContextFactory<SuperStockpileManContext> factory;

        public CategoriesViewModel(IDbContextFactory<SuperStockpileManContext> factory)
        {
            this.factory = factory;
            Categories = [];
        }

        [ObservableProperty]
        private ObservableCollection<CategoryBase> categories;

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            Categories.Clear();

            foreach (
                CategoryBase category in
                context
                .CategoryBases
                .OrderBy(e  => e.Name))
                Categories.Add(category);
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(IsSelected))]
        private async Task RemoveAsync(CategoryBase? category)
        {
            if (category == null) return;

            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            context.Remove(category);
            await context.SaveChangesAsync();

            await LoadAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task AddCategoryAsync()
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            Category category = new();
            context.Add(category);

            await context.SaveChangesAsync();
            await LoadAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(IsSelected))]
        private async Task AddParentCategoryAsync(CategoryBase? category)
        {
            if (category is not null)
            {
                if (category.ParentId is null)
                {
                    using SuperStockpileManContext context = await factory.CreateDbContextAsync();

                    Category newCategory = new();
                    newCategory.Children.Add(category);
                    context.Add(newCategory);

                    await context.SaveChangesAsync();

                    if (newCategory.Id is not 0)
                    {
                        category.ParentId = newCategory.Id;
                        context.Update(category);
                        await context.SaveChangesAsync();
                    }

                    await LoadAsync();
                }
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(IsCategory))]
        private async Task AddChildAsync(CategoryBase? category)
        {
            if (category is not Category category1)
                return;

            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            Category category2 = new()
            {
                ParentId = category1.Id,
            };
            await context
                .Attach(category1)
                .Collection(e => e.Children)
                .LoadAsync();
            context.Add(category2);
            await context.SaveChangesAsync();

            if (category2.Id is not 0)
                foreach (CategoryBase child in category1.Children)
                    child.ParentId = category2.Id;

            category1.Children.Clear();

            await context.SaveChangesAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(IsCategory))]
        private async Task AddSmallestAsync(CategoryBase? categoryBase)
        {
            if (categoryBase is Category category)
            {
                using SuperStockpileManContext context = await factory.CreateDbContextAsync();

                SmallestCategory smallestCategory = new()
                {
                    ParentId = category.Id,
                };

                context.Add(smallestCategory);
                await context.SaveChangesAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(IsSelected))]
        private void Detail(CategoryBase? categoryBase)
        {
            if (categoryBase is Category category)
            {
                WeakReferenceMessenger.Default.Send(new CategoryInvokedMessage(category));
                return;
            }

            if (categoryBase is SmallestCategory smallestCategory)
            {
                WeakReferenceMessenger.Default.Send(new SmallestCategoryInvokedMessage(smallestCategory));
                return;
            }

            return;
        }

        private static bool IsSelected(CategoryBase? category)
        {
            return category is not null;
        }

        private static bool IsCategory(CategoryBase? category)
        {
            return category is Category;
        }
    }
}

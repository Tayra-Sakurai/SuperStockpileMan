using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using SuperStockpileMan.Bus.Contexts;
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

        private static bool IsSelected(CategoryBase? category)
        {
            return category is not null;
        }

        private static bool IsSmallest(CategoryBase? category)
        {
            return category is SmallestCategory;
        }

        private static bool IsCategory(CategoryBase? category)
        {
            return category is Category;
        }
    }
}

// SPDX-FileCopyrightText: 2026 SAKURAI Tayra <tayra_sakurai@icloud.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.ViewModels
{
    public partial class CategoriesViewModel : ObservableRecipient, IRecipient<CategoryAddedMessage>, IRecipient<CategoryRemovedMessage>, IRecipient<SmallestCategoryAddedMessage>, IRecipient<SmallestCategoryRemovedMessage>
    {
        private readonly IDbContextFactory<SuperStockpileManContext> factory;

        public CategoriesViewModel(IDbContextFactory<SuperStockpileManContext> factory)
            : base()
        {
            this.factory = factory;
            Categories = [];
            Messenger.Register<CategoryAddedMessage>(this);
            Messenger.Register<CategoryRemovedMessage>(this);
            Messenger.Register<SmallestCategoryAddedMessage>(this);
            Messenger.Register<SmallestCategoryRemovedMessage>(this);
        }

        ~CategoriesViewModel()
        {
            Messenger.UnregisterAll(this);
        }

        [ObservableProperty]
        private ObservableCollection<Category> categories;
        [ObservableProperty]
        private string searchPhrase = string.Empty;

        private async Task LoadCategoryAsync(Func<CategoryBase, bool> predicate)
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            Categories.Clear();

            Stack<Category> stack = new();
            List<Category> list = [];

            await foreach(
                Category category in
                context
                .Categories
                .AsAsyncEnumerable())
            {
                stack.Push(category);
                list.Insert(0, category);
            }

            while (stack.Count > 0)
            {
                Category current = stack.Pop();

                EntityEntry<Category> entityEntry = context.Entry(current);
                await entityEntry
                    .Collection(e => e.Children)
                    .LoadAsync();

                foreach (CategoryBase categoryBase in current.Children.Where(predicate))
                    if (categoryBase is Category category)
                        stack.Push(category);
            }

            
        }

        private async Task LoadCategoryAsync()
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            Categories.Clear();

            Stack<Category> categories = new();
            List<Category> list = [];

            await foreach (
                Category category in
                context
                .Categories
                .Where(e => e.ParentId == null)
                .AsAsyncEnumerable())
                categories.Push(category);

            while (categories.Count > 0)
            {
                Category current = categories.Pop();
                EntityEntry<Category> entityEntry = context.Entry(current);
                await entityEntry
                    .Collection(e => e.Children)
                    .LoadAsync();
                Debug.WriteLine(entityEntry.State);
                list.Add(current);

                foreach (
                    CategoryBase categoryBase in
                    current.Children)
                    if (categoryBase is Category category)
                        categories.Push(category);
            }

            foreach (
                Category category1 in
                list
                .OrderBy(e => e.Name)
                .ToList())
                Categories.Add(category1);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            await LoadCategoryAsync();
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

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanAddParent))]
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
                Messenger.Send(new CategoryInvokedMessage(category));
                return;
            }

            if (categoryBase is SmallestCategory smallestCategory)
            {
                Messenger.Send(new SmallestCategoryInvokedMessage(smallestCategory));
                return;
            }

            return;
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(IsValidSearchPhrase))]
        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchPhrase))
                return;

            string[] phrases = SearchPhrase.Split(' ');

            await LoadCategoryAsync(e => phrases.Any(p => e.Name.Contains(p)));
        }

        private static bool IsSelected(CategoryBase? category)
        {
            return category is not null;
        }

        private static bool CanAddParent(CategoryBase? category)
        {
            return category is not null && category.ParentId <= 0;
        }

        private static bool IsCategory(CategoryBase? category)
        {
            return category is Category;
        }

        private bool IsValidSearchPhrase()
        {
            return !string.IsNullOrWhiteSpace(SearchPhrase);
        }

        public async void Receive(CategoryAddedMessage message)
        {
            await LoadAsync();
        }

        public async void Receive(CategoryRemovedMessage message)
        {
            await LoadAsync();
        }

        public async void Receive(SmallestCategoryAddedMessage message)
        {
            await LoadAsync();
        }

        public async void Receive(SmallestCategoryRemovedMessage message)
        {
            await LoadAsync();
        }
    }
}

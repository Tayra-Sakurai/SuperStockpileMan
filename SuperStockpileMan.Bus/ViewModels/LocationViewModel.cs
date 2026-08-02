// SPDX-FileCopyrightText: 2026 SAKURAI Tayra <tayra_sakurai@icloud.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.UI.Xaml.Controls;
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
    public partial class LocationViewModel : ObservableValidator
    {
        private readonly IDbContextFactory<SuperStockpileManContext> dbContextFactory;

        private Location location;

        public LocationViewModel(IDbContextFactory<SuperStockpileManContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
            location = new();
            Categories = [];
        }

        [ObservableProperty]
        private ObservableCollection<Category> categories;

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            using SuperStockpileManContext context = await dbContextFactory.CreateDbContextAsync();

            EntityEntry<Location> entityEntry = context.Attach(location);
            await entityEntry
                .Collection(e => e.CategoryBases)
                .LoadAsync();

            Categories.Clear();
            Stack<Category> stack = new();

            await foreach (
                Category category in
                context
                .Categories
                .Where(e => e.ParentId == null)
                .AsAsyncEnumerable())
            {
                Categories.Add(category);
                stack.Push(category);
            }

            while (stack.Count > 0)
            {
                Category current = stack.Pop();
                await context
                    .Entry(current)
                    .Collection(e => e.Children)
                    .LoadAsync();

                foreach (CategoryBase child in current.Children)
                    if (child is Category ctg)
                        stack.Push(ctg);
            }
        }

        public async Task LoadExistingDataAsync(Location location)
        {
            this.location = location;
            OnPropertyChanged(nameof(Name));

            await LoadAsync();
        }

        [RelayCommand(CanExecute = nameof(CanAddRelatedCategory))]
        private void AddRelatedCategory(CategoryBase? category)
        {
            if (category == null)
                return;

            if (location.CategoryBases.Contains(category))
                location.CategoryBases.Add(category);
        }

        private bool CanAddRelatedCategory(CategoryBase? category)
        {
            if (category == null)
                return false;

            if (location.CategoryBases.Contains(category))
                return false;

            return true;
        }

        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages))]
        public string Name
        {
            get => location.Name;
            set
            {
                if (SetProperty(location.Name, value, location, (m, v) => m.Name = v, true))
                {
                    SaveCommand.NotifyCanExecuteChanged();
                }
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            if (HasErrors)
                return;

            using SuperStockpileManContext context = await dbContextFactory.CreateDbContextAsync();

            context.Update(location);
            await context.SaveChangesAsync();
        }

        private bool CanSave()
        {
            ValidateAllProperties();
            return !HasErrors;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task RemoveAsync()
        {
            using SuperStockpileManContext context = await dbContextFactory.CreateDbContextAsync();

            context.Remove(location);
            await context.SaveChangesAsync();

            WeakReferenceMessenger.Default.Send(new LocationRemovedMessage(location));
        }
    }
}

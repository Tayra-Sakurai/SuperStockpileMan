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
        }

        public async Task LoadExistingCategory(Category category)
        {
            this.category = category;

            OnPropertyChanged(nameof(Name));
            RemoveCommand.NotifyCanExecuteChanged();
        }

        [Required]
        public string Name
        {
            get => category.Name;
            set
            {
                if (SetProperty(category.Name, value, category, (m, v) => m.Name = v, true))
                    SaveCommand.NotifyCanExecuteChanged();
            }
        }

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
    }
}

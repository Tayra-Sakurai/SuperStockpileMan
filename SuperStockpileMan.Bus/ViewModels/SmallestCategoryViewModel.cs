// SPDX-FileCopyrightText: 2026 SAKURAI Tayra <tayra_sakurai@icloud.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
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
    public partial class SmallestCategoryViewModel : ObservableValidator
    {
        private readonly IDbContextFactory<SuperStockpileManContext> factory;
        private SmallestCategory smallestCategory;
        
        public SmallestCategoryViewModel(IDbContextFactory<SuperStockpileManContext> factory)
        {
            this.factory = factory;
            smallestCategory = new();
        }

        public async Task LoadExistingValue(SmallestCategory smallestCategory)
        {
            this.smallestCategory = smallestCategory;

            OnPropertyChanged(nameof(Name));

            RemoveCommand.NotifyCanExecuteChanged();
            SaveCommand.NotifyCanExecuteChanged();
            AddCommand.NotifyCanExecuteChanged();
        }

        [Required]
        public string Name
        {
            get => smallestCategory.Name;
            set
            {
                if (SetProperty(smallestCategory.Name, value, smallestCategory, (m, v) => m.Name = v, true))
                {
                    AddCommand.NotifyCanExecuteChanged();
                    SaveCommand.NotifyCanExecuteChanged();
                }
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanRemove))]
        private async Task RemoveAsync()
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            context.Remove(smallestCategory);
            await context.SaveChangesAsync();

            WeakReferenceMessenger.Default.Send(new SmallestCategoryRemovedMessage(smallestCategory));
        }

        private bool CanRemove()
        {
            return smallestCategory.Id > 0;
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanAddOrSave))]
        private async Task AddAsync()
        {
            if (HasErrors)
                return;

            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            context.Add(smallestCategory);
            await context.SaveChangesAsync();

            WeakReferenceMessenger.Default.Send(new SmallestCategoryAddedMessage(smallestCategory));
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanAddOrSave))]
        private async Task SaveAsync()
        {
            if (!HasErrors)
            {
                using SuperStockpileManContext context = await factory.CreateDbContextAsync();

                context.Update(smallestCategory);
                await context.SaveChangesAsync();
            }
        }

        private bool CanAddOrSave()
        {
            ValidateAllProperties();
            return !HasErrors;
        }
    }
}

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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.ViewModels
{
    public partial class ItemsViewModel : ObservableRecipient, IRecipient<ItemAddedMessage>, IRecipient<ItemRemovedMessage>
    {
        private readonly IDbContextFactory<SuperStockpileManContext> factory;

        public ItemsViewModel(IDbContextFactory<SuperStockpileManContext> factory)
            : base()
        {
            this.factory = factory;
            Items = [];

            Messenger.Register<ItemAddedMessage>(this);
            Messenger.Register<ItemRemovedMessage>(this);
        }

        ~ItemsViewModel()
        {
            Messenger.UnregisterAll(this);
        }

        [ObservableProperty]
        private ObservableCollection<Item> items;

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            Items.Clear();

            await foreach 
                (Item item in
                context
                .Items
                .OrderBy(e => e.Id)
                .AsAsyncEnumerable())
                Items.Add(item);
        }

        [RelayCommand(CanExecute = nameof(IsSelected))]
        private static void Detail(Item? item)
        {
            if (item == null)
                return;

            WeakReferenceMessenger.Default.Send(new ItemInvokedMessage(item));
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(IsSelected))]
        private async Task RemoveAsync(Item? item)
        {
            if (item == null)
                return;

            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            context.Remove(item);
            await context.SaveChangesAsync();
            await LoadAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task AddAsync()
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            Item item = new()
            {
                SmallestCategory = context.SmallestCategories.FirstOrDefault(),
                Location = context.Locations.FirstOrDefault(),
            };

            context.Add(item);
            await context.SaveChangesAsync();
        }

        private static bool IsSelected(Item? item)
        {
            return item is not null;
        }

        public async void Receive(ItemAddedMessage message)
        {
            await LoadAsync();
        }

        public async void Receive(ItemRemovedMessage message)
        {
            await LoadAsync();
        }
    }
}

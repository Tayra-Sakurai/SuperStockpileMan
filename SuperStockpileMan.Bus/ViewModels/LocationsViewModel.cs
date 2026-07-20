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
    public partial class LocationsViewModel : ObservableRecipient, IRecipient<LocationAddedMessage>, IRecipient<LocationRemovedMessage>
    {
        private readonly IDbContextFactory<SuperStockpileManContext> factory;

        public LocationsViewModel(IDbContextFactory<SuperStockpileManContext> factory)
            : base()
        {
            this.factory = factory;
            Locations = [];
            Messenger.Register<LocationAddedMessage>(this);
            Messenger.Register<LocationRemovedMessage>(this);
        }

        [ObservableProperty]
        private ObservableCollection<Location> locations;

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            Locations.Clear();

            await foreach (
                Location location in
                context
                .Locations
                .OrderBy(e => e.Name)
                .AsAsyncEnumerable())
                Locations.Add(location);
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(IsSelected))]
        private async Task RemoveAsync(Location? location)
        {
            if (location == null) return;

            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            context.Remove(location);
            await context.SaveChangesAsync();

            await LoadAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task AddAsync()
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            Location location = new();
            context.Add(location);
            await context.SaveChangesAsync();

            await LoadAsync();
        }

        [RelayCommand(CanExecute = nameof(IsSelected))]
        private static void Invoke(Location? location)
        {
            if (location == null)
                return;

            WeakReferenceMessenger.Default.Send(new LocationInvokedMessage(location)); ;
        }

        private static bool IsSelected(Location? location)
        {
            return location is not null;
        }

        public async void Receive(LocationAddedMessage message)
        {
            await LoadAsync();
        }

        public async void Receive(LocationRemovedMessage message)
        {
            await LoadAsync();
        }
    }
}

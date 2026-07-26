// SPDX-FileCopyrightText: 2026 SAKURAI Tayra <tayra_sakurai@icloud.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
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
    public partial class LogsViewModel : ObservableObject
    {
        private readonly IDbContextFactory<SuperStockpileManContext> dbContextFactory;

        public LogsViewModel(IDbContextFactory<SuperStockpileManContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
            Logs = [];
        }

        [ObservableProperty]
        private ObservableCollection<Log> logs;

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            using SuperStockpileManContext context = await dbContextFactory.CreateDbContextAsync();

            Logs.Clear();

            await foreach (
                Log log in
                context
                .Logs
                .OrderBy(log => log.DateTime)
                .ThenBy(log => log.Id)
                .AsAsyncEnumerable())
                Logs.Add(log);
        }
    }
}

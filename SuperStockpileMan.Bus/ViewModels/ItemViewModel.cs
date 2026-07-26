using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SuperStockpileMan.Bus.Contexts;
using SuperStockpileMan.Bus.Models;
using SuperStockpileMan.Bus.Validations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.ViewModels
{
    public partial class ItemViewModel : ObservableValidator
    {
        private readonly IDbContextFactory<SuperStockpileManContext> factory;

        private Item item;

        public ItemViewModel(IDbContextFactory<SuperStockpileManContext> factory)
        {
            this.factory = factory;
            item = new();
            IsDueDateEnabled = false;
            SmallestCategories = [];
            Locations = [];
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task LoadAsync()
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            SmallestCategories.Clear();
            Locations.Clear();

            await foreach (
                SmallestCategory category in
                context
                .SmallestCategories
                .OrderBy(e => e.Id)
                .AsAsyncEnumerable())
                SmallestCategories.Add(category);

            await foreach (
                Location location in
                context
                .Locations
                .OrderBy(e => e.Name)
                .AsAsyncEnumerable())
                Locations.Add(location);
        }

        public async Task LoadExistingValuAsync(Item item)
        {
            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            EntityEntry<Item> entity = context.Attach(item);

            await entity
                .Reference(e => e.SmallestCategory)
                .LoadAsync();

            await entity
                .Reference(e => e.Location)
                .LoadAsync();

            SmallestCategories.Clear();
            Locations.Clear();

            await foreach (
                SmallestCategory smallestCategory in
                context.SmallestCategories
                .OrderBy(e => e.Id)
                .AsAsyncEnumerable())
                SmallestCategories.Add(smallestCategory);

            await foreach (
                Location location in
                context
                .Locations
                .OrderBy(e => e.Name)
                .AsAsyncEnumerable())
                Locations.Add(location);

            this.item = item;

            if (item.DueDate is null)
                IsDueDateEnabled = false;
            else
                IsDueDateEnabled = true;

            OnPropertyChanged(nameof(DueDate));
            OnPropertyChanged(nameof(IsDueDateEnabled));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(PurchaseDate));
            OnPropertyChanged(nameof(SmallestCategories));
            OnPropertyChanged(nameof(SmallestCategory));
            OnPropertyChanged(nameof(Location));
            OnPropertyChanged(nameof(Locations));
        }

        [ObservableProperty]
        private ObservableCollection<SmallestCategory> smallestCategories;

        [ObservableProperty]
        private ObservableCollection<Location> locations;

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            if (HasErrors)
                return;

            using SuperStockpileManContext context = await factory.CreateDbContextAsync();

            context.Update(item);
            await context.SaveChangesAsync();
        }

        private bool CanSave()
        {
            ValidateAllProperties();
            return !HasErrors;
        }

        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(string))]
        public string Name
        {
            get => item.Name;
            set
            {
                if (SetProperty(item.Name, value, item, (m, v) => m.Name = v, true))
                {
                    SaveCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public string Description
        {
            get => item.Description;
            set => SetProperty(item.Description, value, item, (m, v) => m.Description = v);
        }

        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(string))]
        [PastDateTimeOffsetValidation(ErrorMessageResourceName = "MustBePastMessage", ErrorMessageResourceType = typeof(string))]
        public DateTimeOffset PurchaseDate
        {
            get => item.PurchaseDate;
            set
            {
                if (SetProperty(item.PurchaseDate, value, item, (m, v) => m.PurchaseDate = v, true))
                {
                    SaveCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private bool isDueDateEnabled;

        public bool IsDueDateEnabled
        {
            get => isDueDateEnabled;
            set
            {
                if (SetProperty(ref isDueDateEnabled, value))
                    DueDate = isDueDateEnabled ? DateTimeOffset.Now : null;
            }
        }

        public DateTimeOffset? DueDate
        {
            get => item.DueDate;
            set => SetProperty(item.DueDate, value, item, (m, v) => m.DueDate = v);
        }

        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(string))]
        public SmallestCategory SmallestCategory
        {
            get => SmallestCategories.First(e => e.Id == item.SmallestCategoryId);
            set
            {
                if (SetProperty(item.SmallestCategoryId, value.Id, item, (m, v) => m.SmallestCategoryId = v, true))
                    SaveCommand.NotifyCanExecuteChanged();
            }
        }

        [Required(ErrorMessageResourceName = "RequiredMessage", ErrorMessageResourceType = typeof(string))]
        public Location Location
        {
            get => Locations.First(l => l.Id == item.LocationId);
            set
            {
                if (SetProperty(item.LocationId, value.Id, item, (m, v) => m.LocationId = v, true))
                    SaveCommand.NotifyCanExecuteChanged();
            }
        }
    }
}

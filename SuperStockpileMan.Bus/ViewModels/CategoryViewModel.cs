using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using SuperStockpileMan.Bus.Contexts;
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
        }

        [Required]
        public string Name
        {
            get => category.Name;
            set => SetProperty(category.Name, value, category, (m, v) => m.Name = v, true);
        }
    }
}

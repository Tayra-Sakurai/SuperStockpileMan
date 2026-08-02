using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SuperStockpileMan.Bus.ViewModels;

namespace SuperStockpileMan.Views
{
    public sealed partial class ItemsView : Page
    {
        private ItemsViewModel? viewModel;

        public ItemsView()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            viewModel = Ioc.Default.GetRequiredService<ItemsViewModel>();

            await viewModel.LoadAsync();
        }
    }
}

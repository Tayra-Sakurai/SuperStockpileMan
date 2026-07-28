using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SuperStockpileMan.Bus.Messages;
using SuperStockpileMan.Bus.Models;
using SuperStockpileMan.Bus.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SuperStockpileMan.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SmallestCategoryView : Page, IRecipient<SmallestCategoryRemovedMessage>
    {
        private SmallestCategoryViewModel? viewModel;

        public SmallestCategoryView()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            viewModel = Ioc.Default.GetRequiredService<SmallestCategoryViewModel>();

            if (e.Parameter is SmallestCategory smallestCategory)
            {
                await viewModel.LoadExistingValueAsync(smallestCategory);
            }

            WeakReferenceMessenger.Default.Register(this);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        public void Receive(SmallestCategoryRemovedMessage message)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
                Frame.Navigate(typeof(CategoriesView));
        }
    }
}

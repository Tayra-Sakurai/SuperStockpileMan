using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SuperStockpileMan.Bus.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using SuperStockpileMan.Bus.Messages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SuperStockpileMan.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CategoriesView : Page, IRecipient<CategoryInvokedMessage>, IRecipient<CategoryRemovedMessage>, IRecipient<SmallestCategoryInvokedMessage>
{
    private CategoriesViewModel? categoriesViewModel;

    public CategoriesView()
    {
        InitializeComponent();
    }

    ~CategoriesView()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        categoriesViewModel = Ioc.Default.GetRequiredService<CategoriesViewModel>();

        await categoriesViewModel.LoadAsync();

        WeakReferenceMessenger.Default.Register<CategoryInvokedMessage>(this);
        WeakReferenceMessenger.Default.Register<CategoryRemovedMessage>(this);
        WeakReferenceMessenger.Default.Register<SmallestCategoryInvokedMessage>(this);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);

        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    public void Receive(CategoryInvokedMessage message)
    {
        Frame.Navigate(typeof(CategoryView), message.Value);
    }

    public async void Receive(CategoryRemovedMessage message)
    {
        if (categoriesViewModel != null)
            await categoriesViewModel.LoadAsync();
    }

    public void Receive(SmallestCategoryInvokedMessage message)
    {
        Frame.Navigate(typeof(SmallestCategoryView), message.Value);
    }
}

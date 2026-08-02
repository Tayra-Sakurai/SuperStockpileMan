// SPDX-FileCopyrightText: 2026 SAKURAI Tayra <tayra_sakurai@icloud.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.ApplicationModel.Resources;
using SuperStockpileMan.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SuperStockpileMan
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            SuperNav.ItemInvoked += SuperNav_ItemInvoked;
            MainFrame.Navigated += MainFrame_Navigated;
            SuperNav.BackRequested += SuperNav_BackRequested;
        }

        private void SuperNav_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            SuperNav.IsBackEnabled = MainFrame.CanGoBack;
        }

        private void SuperNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            ResourceLoader resourceLoader = new();

            NavigationViewItem? invokedItem = sender.SelectedItem as NavigationViewItem;
            sender.Header = invokedItem?.Content ?? string.Empty;

            if (invokedItem?.Tag is not null)
                MainFrame.Navigate(Type.GetType($"SuperStockpileMan.Views.{invokedItem!.Tag}"));
        }
    }
}


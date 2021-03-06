﻿using PSRT.Astra.Models;
using PSRT.Astra.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace PSRT.Astra.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _ViewModel;

        public MainWindow(string pso2BinPath, UpdateChecker.UpdateInformation updateInformation)
        {
            InitializeComponent();

            _ViewModel = new MainWindowViewModel(pso2BinPath, updateInformation);
            DataContext = _ViewModel;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await _ViewModel.InitializeAsync();
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            await _ViewModel.DestroyAsync();
        }

        private void _Log_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is ListView list)
            {
                if (list.Items.Count > 0)
                    list.ScrollIntoView(list.Items[list.Items.Count - 1]);
            }
        }

        private async void _SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!await _ViewModel.CanOpenSettingsAsync())
            {
                App.Logger.Warning(nameof(MainWindow), "User settings file does not exists, please run the game once to generate it");
                MessageBox.Show("User settings file does not exists, please run the game once to generate it", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new PSO2OptionsWindow();
            window.Owner = this;
            window.ShowDialog();
        }

        private void _AstraSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new OptionsWindow(_ViewModel.InstallConfiguration);
            window.Owner = this;
            window.ShowDialog();
        }

        private void _AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AboutWindow();
            window.Owner = this;
            window.ShowDialog();
        }

        private void _Hyperlink_RequestNavigateBrowser(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        private void _PhaseControl_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (e.Property == PhaseControl.PhaseStateProperty)
                if (sender is FrameworkElement element)
                    element.BringIntoView();
        }

        private async void _UploadErrorLogButton_Click(object sender, RoutedEventArgs e)
        {
            await _ViewModel.CancelAndUploadErrorAsync();
        }
    }
}

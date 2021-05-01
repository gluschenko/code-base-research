﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using CodeBase.Client.Pages;
using CodeBase.Domain;
using CodeBase.Domain.Models;
using CodeBase.Domain.Services;

namespace CodeBase.Client.Views
{
    public partial class MainWindow : Window
    {
        private readonly DataManager<AppData> _dataManager;
        private readonly AppData _appData;
        private readonly Context _context;
        private readonly HashSet<Page> _activePages;

        public MainWindow()
        {
            InitializeComponent();

            _activePages = new HashSet<Page>();
            _dataManager = new DataManager<AppData>("prefs.json");

            try
            {
                _appData = _dataManager.Load();
            }
            catch (Exception ex)
            {
                MessageHelper.Error(ex.Message);
                Close();
            }

            _context = new Context
            {
                AppData = _appData,
            };

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;

            FillSidebarMenu();
            Navigate(typeof(MainPage));
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Width = _appData.WindowWidth ?? Width;
            Height = _appData.WindowHeight ?? Height;
            WindowState = _appData.WindowState ?? WindowState;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _appData.WindowWidth = Width;
            _appData.WindowHeight = Height;
            _appData.WindowState = WindowState;

            try
            {
                _dataManager.Save(_appData);
            }
            catch (Exception ex)
            {
                MessageHelper.Error(ex.Message);
            }
        }

        private void FillSidebarMenu()
        {
            var pages = GetType().Assembly.GetTypes()
                .Where(x => x.IsSubclassOf(typeof(Page)))
                .Select(x => new
                {
                    Type = x,
                    Descriptor = x.GetCustomAttributes().OfType<PageDescriptorAttribute>().FirstOrDefault(),
                })
                .Where(x => x.Descriptor is not null)
                .OrderBy(x => x.Descriptor.Order)
                .ToArray();

            SidebarMenu.Items.Clear();
            SidebarMenu.ItemsSource = pages.Select(x => new PageLink
            {
                Title = x.Descriptor.Title,
                PageTypeName = x.Type.Name,
            });
        }

        private void SidebarItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pageTypeName = (sender as FrameworkElement).Tag?.ToString() ?? "";

            var pageType = GetType().Assembly.GetTypes().FirstOrDefault(x => x.Name == pageTypeName);

            if (pageType is null)
            {
                return;
            }

            Navigate(pageType);
        }

        private void Navigate(Type pageType)
        {
            var pageDescriptor = pageType.GetCustomAttributes()
                .OfType<PageDescriptorAttribute>().FirstOrDefault();

            if (pageDescriptor is null)
            {
                return;
            }

            var pageItem = _activePages.FirstOrDefault(x => x.GetType() == pageType);

            if (pageDescriptor.Lifetime == PageLifetime.Transient || pageItem == null)
            {
                if (pageItem is not null)
                {
                    _activePages.Remove(pageItem);
                }

                pageItem = (Page)Activator.CreateInstance(pageType, args: _context);
                _activePages.Add(pageItem);
            }

            PageFrame.Navigate(pageItem);
        }
    }
}
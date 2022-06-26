﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wishmaster.Services;
using Wishmaster.ViewModels;

namespace Wishmaster.Views.Pages
{
    public partial class SpaceListPage : Page
    {

        private readonly ISpaceService _spaceService;

        private readonly ObservableCollection<SpaceItemViewModel> _spaces;

        public SpaceListPage(ISpaceService spaceService)
        {
            InitializeComponent();

            _spaceService = spaceService;

            _spaces = new ObservableCollection<SpaceItemViewModel>();
            SpacesList.Items.Clear();
            SpacesList.ItemsSource = _spaces;

            _spaces.Add(new SpaceItemViewModel { Name = "123" });
        }

        private void SpaceItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var uid = GetUidFromTag(sender);
            _spaces.First().Name = uid.ToString();
            SpacesList.Items.Refresh();
        }

        private void SpaceEdit_Click(object sender, RoutedEventArgs e)
        {
            var uid = GetUidFromTag(sender);

        }

        private void SpaceDelete_Click(object sender, RoutedEventArgs e)
        {
            var uid = GetUidFromTag(sender);

        }

        private static Guid GetUidFromTag(object sender)
        {
            if (sender is FrameworkElement item)
            {
                return Guid.Parse(item.Tag?.ToString() ?? "");
            }

            throw new Exception();
        }
    }
}
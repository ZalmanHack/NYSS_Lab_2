using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NYSS_Lab_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private enum VisibleColTypes { Minimazed, Normal, Compare }
        private static List<string> showedTypes = new List<string> { "Сокращенно", "Полностью", "Сравнение" };
        private static VisibleColTypes visibleColType;
        public SourseDataController controller;

        public MainWindow()
        {
            InitializeComponent();
            ShowAs.ItemsSource = showedTypes;
            controller = new SourseDataController();
            controller.RowsLimitEvent += SetEnablePagination;
            controller.Load();
        }

        protected void ReloadData(List<SourseData> data)
        {
            Table.ItemsSource = null;
            Table.ItemsSource = data;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                SourseData data = e.AddedItems[0] as SourseData;
                Id.Text = data.Id;
                Name.Text = data.Name;
                Description.Text = data.Description;
                Sourse.Text = data.Sourse;
                Target.Text = data.Target;
                Confidentiality.Text = data.Confidentiality;
                Integrity.Text = data.Integrity;
                Access.Text = data.Access;
            }
        }

        private void Table_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if((bool)ShowChanged.IsChecked)
            {
                SourseData row = e.Row.DataContext as SourseData;
                if (row.RecordStatus == RecordStatuses.New)
                {
                    e.Row.Background = new SolidColorBrush(Colors.Green);
                }
                else if(row.RecordStatus == RecordStatuses.Changed)
                {
                    e.Row.Background = new SolidColorBrush(Colors.Orange);
                }
                else if (row.RecordStatus == RecordStatuses.Deleted)
                {
                    e.Row.Background = new SolidColorBrush(Colors.Red);
                }
                else if (row.RecordStatus == RecordStatuses.Actual)
                {
                    e.Row.Background = new SolidColorBrush(Colors.White);
                }
            }
        }
        private void Table_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var desc = e.PropertyDescriptor as PropertyDescriptor;
            var attrib = desc.Attributes[typeof(ColumnNameAttribute)];
            if (attrib != null)
                e.Column.Header = (attrib as ColumnNameAttribute).Name;

            if (visibleColType == VisibleColTypes.Normal)
            {
                attrib = desc.Attributes[typeof(ColNormalAttribute)];
                InfoPanel.Visibility = Visibility.Hidden;
                InfoPanel.Width = 0;
            }

            else if (visibleColType == VisibleColTypes.Minimazed)
            {
                attrib = desc.Attributes[typeof(ColMinimazeAttribute)];
                InfoPanel.Visibility = Visibility.Visible;
                InfoPanel.Width = 300;
            }
            else if (visibleColType == VisibleColTypes.Compare)
            {
                attrib = desc.Attributes[typeof(ColCompareAttribute)];
                InfoPanel.Visibility = Visibility.Hidden;
                InfoPanel.Width = 0;
            }

            if (attrib != null)
                e.Column.Visibility = Visibility.Visible;
            else
                e.Column.Visibility = Visibility.Hidden;
        }

        private void ShowAs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int item = (sender as ComboBox).SelectedIndex;
            if (Enum.IsDefined(typeof(VisibleColTypes), item))
                visibleColType = (VisibleColTypes)item;
            ReloadData(controller.Load());
        }

        private void ShowAs_Loaded(object sender, RoutedEventArgs e)
        {
            ShowAs.SelectedIndex = 0;
        }

        private void SetEnablePagination(RowsLinits value)
        {
            if (value == RowsLinits.End)
            {
                Next.Visibility = Visibility.Collapsed;
                Back.Visibility = Visibility.Visible;
            }
            else if (value == RowsLinits.Middle)
            {
                Next.Visibility = Visibility.Visible;
                Back.Visibility = Visibility.Visible;
            }
            else if (value == RowsLinits.Start)
            {
                Next.Visibility = Visibility.Visible;
                Back.Visibility = Visibility.Collapsed;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {

            ReloadData(controller.Back());
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            ReloadData(controller.Next());
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Updfrequency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Updfrequency_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Timers;
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
        private static List<string> showedTime = new List<string> { "1 мин", "30 мин", "1 час" };
        private static VisibleColTypes visibleColType;
        public SourseDataController controller;
        private Timer timerUpdate;
        public List<SourseData> Data = new List<SourseData>();

        public MainWindow()
        {
            InitializeComponent();
            ShowAs.ItemsSource = showedTypes;
            Updfrequency.ItemsSource = showedTime;
            controller = new SourseDataController();
            controller.RowsLimitEvent += SetEnablePagination;
            controller.DataLoader.InfoEvent += InfoLabel_SetText;
            timerUpdate = new Timer(30000);
            timerUpdate.Elapsed += Timeout;
            timerUpdate.Start();
        }

        // БЫЛА ЗАГРУЗКА И ОБНОВЛЕНИЕ ЧЕРЕЗ ТАЙМАУТ,  
        // НО Table.Items.Refresh(); НИКОГДА НЕ СРАБАТЫВАЛ ИЗ ТАЙМАУТА.
        // В ДЕБАГЕ ПРОСТО ПРОПУСКАЕТСЯ ЭТОТ МЕТОД
        // КОД:
        // Data = controller.Download();
        // ReloadData();
        private void Timeout(Object source, ElapsedEventArgs e)
        {
            MessageBox.Show("Пора обновить данные.\nДля этого нажмите кнопку \"обновить\"", "My App", MessageBoxButton.OK, MessageBoxImage.Question, MessageBoxResult.No);
        }

        protected void ReloadData()
        {
            Table.ItemsSource = Data;
            Table.Items.Refresh();
        }

        private void InfoLabel_SetText(string value)
        {
            InfoLabel.Content = value;
        }

        private void Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                SourseData data = e.AddedItems[0] as SourseData;
                Id.Text = data.Id;
                NameD.Text = data.Name;
                Description.Text = data.Description;
                Sourse.Text = data.Sourse;
                Target.Text = data.Target;
                Confidentiality.Text = data.Confidentiality;
                Integrity.Text = data.Integrity;
                Access.Text = data.Access;
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
            Data = controller.Load();
            ReloadData();
        }

        private void Updfrequency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            timerUpdate.Stop();
            int item = (sender as ComboBox).SelectedIndex;
            if (item == 0)
                timerUpdate.Interval = 60_000;
            if (item == 1)
                timerUpdate.Interval = 1_800_000;
            if (item == 2)
                timerUpdate.Interval = 3_200_000;
            timerUpdate.Start();
        }

        private void ShowAs_Loaded(object sender, RoutedEventArgs e)
        {
            ShowAs.SelectedIndex = 0;
        }

        private void Updfrequency_Loaded(object sender, RoutedEventArgs e)
        {
            Updfrequency.SelectedIndex = 0;
        }

        private void SetEnablePagination(RowsLinits value)
        {
            if (value == RowsLinits.End)
            {
                Next.IsEnabled = false;
                Back.IsEnabled = true;
            }
            else if (value == RowsLinits.Middle)
            {
                Next.IsEnabled = true;
                Back.IsEnabled = true;
            }
            else if (value == RowsLinits.Start)
            {
                Next.IsEnabled = true;
                Back.IsEnabled = false;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Data = controller.Back();
            ReloadData();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Data = controller.Next();
            ReloadData();
        }

        private void Сompare_Checked(object sender, RoutedEventArgs e)
        {
            controller.SourseType = SourseTypes.Old;
            Data = controller.Load();
            ReloadData();
        }

        private void Сompare_Unchecked(object sender, RoutedEventArgs e)
        {
            controller.SourseType = SourseTypes.New;
            Data = controller.Load();
            ReloadData();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Data = controller.Download();
            ReloadData();
        }

        private void Table_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if(e.Row.GetIndex() % 2 == 0)
            {
                e.Row.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                e.Row.Background = new SolidColorBrush(Colors.WhiteSmoke);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            controller.Save();
        }
    }
}

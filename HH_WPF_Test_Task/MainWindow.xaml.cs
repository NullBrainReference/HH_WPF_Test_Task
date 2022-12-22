using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Threading;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections.Specialized;

namespace HH_WPF_Test_Task
{
    public partial class MainWindow : Window
    {
        private UrlsCollector urlsCollector;
        private CancellationTokenSource tokenSource;

        public MainWindow()
        {
            InitializeComponent();
            urlsCollector = new UrlsCollector();
            tokenSource = new CancellationTokenSource();

            sum_a_label.Content = "";
            taskLable.Content = "";

            openfileMenuItem.Click += new RoutedEventHandler(menuOpen_Click);
        }

        private async void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            ShowLoading();

            await urlsCollector.ReadUrls_from_File(token);
            await urlsCollector.CountTags(token);
            await Task.Run(urlsCollector.SortUrlsAction());

            ShowUrls(urlsCollector.GetMax_a_Count());
            HideLoading();
        }

        private void ShowLoading()
        {
            taskLable.Content = "loading";
            cancelButton.Visibility = Visibility.Visible;
            cancelButton.Content = "cancel";
        }

        private void HideLoading()
        {
            taskLable.Content = "";
            cancelButton.Visibility = Visibility.Hidden;
        }

        private void ShowUrls(int max)
        {
            urlsList.Items.Clear();
            foreach (MyUrl url in urlsCollector.Urls)
            {
                ListBoxItem item = new ListBoxItem();
                if(url.IsUnCounted)
                    item.Content = url.Url + " :Tags not counted";
                else
                    item.Content = url.Url + " - count <a>: " + url.ATagsCount;

                if (url.ATagsCount == max) {
                    item.Background = new SolidColorBrush(Colors.Red);
                }
                urlsList.Items.Add(item);
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
            cancelButton.Content = "canceling";
        }

        private void xlsxHelper_Click(object sender, RoutedEventArgs e)
        {
            DataFormatHelpers.xlsxHelp();
        }
    }
}

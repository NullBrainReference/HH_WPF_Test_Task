using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace HH_WPF_Test_Task
{
    public class UrlsCollector
    {
        private List<MyUrl> urls;
        private bool isLoading = false;

        public List<MyUrl> Urls { get { return urls; } }
        public bool IsLoading { get { return isLoading; } }

        public UrlsCollector()
        {
            urls = new List<MyUrl>();
        }

        public void SetLoading() { isLoading = true; }

        private void AddUrl(string url)
        {
            MyUrl myUrl = new MyUrl(url);
            urls.Add(myUrl);
        }

        public async Task CountTags(CancellationToken token)
        {
            foreach (var url in urls)
            {
                if (token.IsCancellationRequested) return;
                await url.count_a_Tags();
            }
        }

        public Action SortUrlsAction()
        {
            return new Action(() => {
                urls.Sort((x, y) => y.ATagsCount.CompareTo(x.ATagsCount));
            });
        }

        public int GetUrlsTagSum()
        {
            int sum = 0;

            foreach (MyUrl url in urls) sum += url.ATagsCount;

            return sum;
        }

        public int GetMax_a_Count()
        {
            int max = 0;
            foreach(MyUrl url in urls)
            {
                if(url.ATagsCount > max) max = url.ATagsCount;
            }
            return max;
        }

        public async Task ReadUrls_from_File(CancellationToken token)
        {
            string path = "";
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Microsoft Excel|*.xlsx;";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                path = dialog.FileName;
            }

            isLoading = true;

            string[] subPath = path.Split('.');
            string format = subPath[subPath.Length - 1];
            switch (format)
            {
                case "xlsx":
                    await ReadXLSX(path, token);
                    break;
                //Add more file formats;
            }
        }

        public async Task ReadXLSX(string path, CancellationToken token)
        {
            Action action = () =>
            {
                Excel.Application excelFile = new Excel.Application().Application;

                Excel.Workbook workbook = excelFile.Workbooks.Open(path);
                Excel.Worksheet worksheet = workbook.Worksheets[1];

                Excel.Range range = worksheet.Rows[1];

                try
                {
                    string urlIndex = range.Find("Urls", Type.Missing, Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart).Address;
                    urlIndex = urlIndex.Split('$')[1];

                    range = worksheet.Columns[1];
                    string lastIndexS = range.Find("", Type.Missing, Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart).Address;
                    int lastIndex = Convert.ToInt32(lastIndexS.Split('$')[2]);


                    for (int i = 2; i < lastIndex; i++)
                    {
                        if (token.IsCancellationRequested) return;

                        Excel.Range cell = worksheet.Range[urlIndex + i.ToString()];
                        string url = Convert.ToString(cell.Value);

                        AddUrl(url);
                    }
                    workbook.Close();
                    excelFile.Quit();

                    isLoading = false;
                }
                catch (Exception ex)
                {
                    string message = "Wrong xlsx file format: " + ex.Message;
                    MessageBoxResult result = MessageBox.Show(message, "Error", MessageBoxButton.OK);
                    if (result == MessageBoxResult.Yes)
                    {
                        workbook.Close();
                        excelFile.Quit();
                        return;
                    }
                }

            };
            await Task.Run(action);
        }
    }
}

using System.Windows;

namespace HH_WPF_Test_Task
{
    public class DataFormatHelpers
    {
        public static void xlsxHelp()
        {
            string message = "File should have title 'Urls' in the first row." +
                " Data should be placed under title, with no empty cells.";
            MessageBoxResult result = MessageBox.Show(message, "Help", MessageBoxButton.OK);
            if (result == MessageBoxResult.Yes)
            {
                return;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HandwritingInstituteBillingSystem.CommonViewHandlers;
using HandwritingInstituteBillingSystem.Logic.Configs;
using HandwritingInstituteBillingSystem.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace HandwritingInstituteBillingSystem.Views
{
    /// <summary>
    /// Interaction logic for NewForm.xaml
    /// </summary>
    public partial class NewForm : MetroWindow
    {
        public NewForm()
        {
            InitializeComponent();
            NewFormViewHandler.OnCloseForm += OnCloseWindow;
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var newEntryViewModel = Grid1.DataContext as NewEntryViewModel;
            if (newEntryViewModel != null)
                newEntryViewModel.StartValidation = true;
        }


        private void OnCloseWindow(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9]");
            return reg.IsMatch(str);

        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (sender as TextBox);
            var v = textBox.Text;
            decimal d = 0;
            if (decimal.TryParse(v, out d))
            {
                return;
            }
            textBox.ToolTip = "Enter only decimal values";
            textBox.Text = String.Empty;
        }
    }
}

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
using HandwritingInstituteBillingSystem.ViewModels;
using MahApps.Metro.Controls;

namespace HandwritingInstituteBillingSystem.Views
{
    /// <summary>
    /// Interaction logic for PartPayment.xaml
    /// </summary>
    public partial class PartPayment : MetroWindow
    {
        private NewEntryViewModel value;

        public PartPayment(NewEntryViewModel value)
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
            var newEntryViewModel = Grid1.DataContext as NewEntryViewModel;
            newEntryViewModel.Set(value);
            InstallmentHandler.OnCloseForm += OnClose;
        }

        private void OnClose(object sender, EventArgs e)
        {
            Close();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var newEntryViewModel = Grid1.DataContext as NewEntryViewModel;
            if (newEntryViewModel != null)
                newEntryViewModel.StartValidation = true;
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

using System;
using HandwritingInstituteBillingSystem.CommonViewHandlers;
using HandwritingInstituteBillingSystem.ViewModels;
using MahApps.Metro.Controls;

namespace HandwritingInstituteBillingSystem.Views
{
    /// <summary>
    /// Interaction logic for ViewPrintForm.xaml
    /// </summary>
    public partial class ViewPrintForm : MetroWindow
    {
        public ViewPrintForm(NewEntryViewModel data)
        {
            InitializeComponent();
            var newEntryViewModel = Grid1.DataContext as NewEntryViewModel;
            newEntryViewModel.SetForPrint(data);
            PrintB.Command = data.PrintCommand;
            ViewPrintViewHandler.OnCloseForm += OnCloseRequest;
        }

        private void OnCloseRequest(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public enum Mode
    {
        PartPaymentMode,
        ViewAndPrint
    }
}

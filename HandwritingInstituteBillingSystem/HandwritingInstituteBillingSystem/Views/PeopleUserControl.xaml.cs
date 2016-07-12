using System;
using System.Windows;
using System.Windows.Controls;
using HandwritingInstituteBillingSystem.CommonViewHandlers;
using HandwritingInstituteBillingSystem.Logic;
using HandwritingInstituteBillingSystem.ViewModels;

namespace HandwritingInstituteBillingSystem.Views
{
    /// <summary>
    /// Interaction logic for PeopleUserControl.xaml
    /// </summary>
    public partial class PeopleUserControl : UserControl
    {
        public PeopleUserControl()
        {
            InitializeComponent();
            this.Loaded += OnLoadedFully;
            InstallmentHandler.ShowInstallmentPaymentForm += OnShowInstallment;
        }

        private void OnLoadedFully(object sender, RoutedEventArgs e)
        {
            var peopleViewModel = (Grid1.DataContext as PeopleViewModel);
            peopleViewModel.FilterText = string.Empty;
            searchBox.Text = string.Empty;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var newForm = new NewForm();
            newForm.ShowDialog();
        }

        private void View_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var peopleViewModel = (Grid1.DataContext as PeopleViewModel);
            if (peopleViewModel == null || peopleViewModel.SelectedUserDetails == null)
            {
                return;
            }
            var userDetails = peopleViewModel.GetSelectedData();
            var newForm = new ViewPrintForm(userDetails);
            newForm.ShowDialog();
        }
        
        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var peopleViewModel = (Grid1.DataContext as PeopleViewModel);
            if (peopleViewModel == null)
            {
                return;
            }

            peopleViewModel.FilterText = searchBox.Text;
        }

        private void OnShowInstallment(object sender, long e)
        {
            var value = Store.Get().GetLatest(e);
            value.AmountPaid = 0;
            BillNoGenerator.IncrementBillSequence();
            value.BillNo = BillNoGenerator.Get();
            var partPayment = new PartPayment(value);
            partPayment.ShowDialog();
        }
    }
}

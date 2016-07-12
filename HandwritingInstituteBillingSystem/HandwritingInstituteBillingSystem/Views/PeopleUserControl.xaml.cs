using System.Windows.Controls;
using HandwritingInstituteBillingSystem.Logic;
using HandwritingInstituteBillingSystem.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

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
    }
}

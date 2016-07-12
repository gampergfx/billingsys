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
    /// Interaction logic for ViewPrintForm.xaml
    /// </summary>
    public partial class ViewPrintForm : MetroWindow
    {
        public ViewPrintForm(NewEntryViewModel data)
        {
            InitializeComponent();
            Grid1.DataContext = data;
            PrintB.Command = data.PrintCommand;
            ViewPrintViewHandler.OnCloseForm += OnCloseRequest;
        }

        private void OnCloseRequest(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

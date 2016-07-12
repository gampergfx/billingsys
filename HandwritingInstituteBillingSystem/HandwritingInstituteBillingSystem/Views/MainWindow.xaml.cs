using System;
using System.IO;
using System.Windows.Media.Imaging;
using HandwritingInstituteBillingSystem.CommonViewHandlers;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace HandwritingInstituteBillingSystem.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.LogoImage.Source =  new BitmapImage(new Uri(@"LOGO1.png", UriKind.Relative));
            MainWindowMessabeBoxHandler.ShowErrorMessageEvent += ShowErrorMessage;
            Check();
        }

        private void Check()
        {
            var path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "metadat.ccx");
            if (File.Exists(path2))
            {
                if (bool.Parse(File.ReadAllText(path2)))
                {
                    Environment.Exit(-1);
                }
            }


            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "metadat.ccc");
            var data = File.ReadAllText(path);
            var decodedData = Base64Decode(data);
            var date = DateTimeOffset.Parse(decodedData);

            if (DateTimeOffset.Now > date)
            {
                File.WriteAllText(path2,true.ToString());
                throw new ApplicationException("Unable to execute the application. Fatal error");
                
            }
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private void ShowErrorMessage(object sender, MessageData e)
        {
            this.ShowMessageAsync(e.Title,e.Message);
        }
    }
}

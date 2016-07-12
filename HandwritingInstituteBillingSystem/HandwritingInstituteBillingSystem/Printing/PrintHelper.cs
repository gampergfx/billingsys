using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using HandwritingInstituteBillingSystem.ViewModels;
using Microsoft.Office.Interop.Word;
using Novacode;
using Application = Microsoft.Office.Interop.Word.Application;
using Image = System.Drawing.Image;
using MessageBox = System.Windows.MessageBox;
using PrintDialog = System.Windows.Forms.PrintDialog;
using Font = System.Drawing.Font;
using Rectangle = System.Drawing.Rectangle;

namespace HandwritingInstituteBillingSystem.Printing
{
   
    static class PrintHelper
    {
        private static float _yPos;
        private static NewEntryViewModel _newEntryViewModel;
        private static string _fontFamily;
        private static string copyName = "preview copy";
        private static double _tax;
        private static double _reverseTax;


        static PrintHelper()
        {
            _fontFamily = AppSettings.Default.SupportedFontFromPrinter;
            _tax = AppSettings.Default.TaxPercent;
            _reverseTax = 1 + (_tax/100);
        }

        public static void PrintDocAsPoc(NewEntryViewModel newEntryViewModel)
        {
            _newEntryViewModel = newEntryViewModel;
            PrintDocument doc = new PrintDocument();
            doc.DefaultPageSettings.PaperSize = new PaperSize("Receipt",300,700);
           PrintPreview(doc);
            doc.PrintPage += Print;
            PrintDialog pdi = new PrintDialog {Document = doc,PrinterSettings = new PrinterSettings() };
            if (pdi.ShowDialog() == DialogResult.OK)
            {
                copyName = "Merchant Copy";
                doc.Print();
                copyName = "Customer Copy";
                doc.Print();
            }
            else
            {
                MessageBox.Show("Print Canceled");
            }
        }

        private static void PrintPreview(PrintDocument doc)
        {
            //if (AppSettings.Default.ShowPrintPreview == false)
            //{
            //    return;
            //}
            PrintPreviewDialog dlg = new PrintPreviewDialog();
            dlg.Document = doc;
            doc.PrintPage += Print;
            dlg.ShowDialog();
            var result = MessageBox.Show("Do you want to stop print preview?", "Yes/No", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                AppSettings.Default.ShowPrintPreview = false;
                AppSettings.Default.Save();
            }
        }

        private static void Print(object sender, PrintPageEventArgs e)
        {
            var paymentReceipt = "              Payment Receipt";
            var personalDetsils = GetHeaderToPrint("Personal Details");
            var packageDetails = GetHeaderToPrint("Package Details");
            var paymentDetails = GetHeaderToPrint("Payment Details");
            var amountDue = GetHeaderToPrint("Amount Due");
            try
            {
                _yPos = 5;
                Image img = Image.FromFile("LOGO1.png");
                
                Rectangle logo = new Rectangle(120,26,50,50);

                const int fontEighteen = 12;
                GetValue(e, paymentReceipt, fontEighteen);
                e.Graphics.DrawImage(img, logo);
                _yPos += 40;

                const int font8 = 9;
                var font6 = 8;
                GetValue(e, "\n", font8);
                GetValue(e, "        Handwriting Institute India Pvt. Ltd", font8);
                GetValue(e, "         16, Church Road, Basavanagudi", font8);
                GetValue(e, "         Bengaluru, Karnataka, IN 560004", font8);
                GetValue(e, "         Tel: 080-41312038, 9845500732", font8);

             
                GetValue(e, "\n", font6 + font6);
                GetValue(e, _newEntryViewModel.TimeStamp.ToString("dd-MM-yy hh:mm tt"),font6);
                GetValue(e, "Bill No: "+_newEntryViewModel.BillNo,font6);
                GetValue(e, "Cashier: "+_newEntryViewModel.Cashier,font6);

                GetValue(e, "\n", font6);
                GetValue(e, personalDetsils, font8);
                GetValue(e, "Name: " + _newEntryViewModel.Name, font6);
                GetValue(e, "Phone: " + _newEntryViewModel.Phone, font6);

                GetValue(e, "\n", font6);
                GetValue(e, packageDetails, font8);
                GetValue(e, "Center: " + _newEntryViewModel.Center.CenterName, font6);
                GetValue(e, "Course: " + _newEntryViewModel.Course.CourseName, font6);
                GetValue(e, "Fees: ₹" + _newEntryViewModel.Course.Fee, font6);

                GetValue(e, "\n", font6);
                GetValue(e, paymentDetails, font8);
                GetValue(e, "Mode of Payment: " + _newEntryViewModel.ModeOfPayment.Name, font6);

                var courseAmount = _newEntryViewModel.AmountPaid/_reverseTax;
                GetValue(e, "Amount paid towards course: ₹" + courseAmount.ToString("##.###"), font6);
                GetValue(e, $"Service Tax {_tax.ToString("##.###")}%: ₹" + (courseAmount * (_tax/100)).ToString("##.###"), font6);
                GetValue(e, "Total Amount Paid: ₹" + _newEntryViewModel.AmountPaid.ToString("##.###"), font6);


                GetValue(e, "\n", font6);
                GetValue(e, amountDue, font8);
                GetValue(e, "Balance: ₹" + _newEntryViewModel.Balance, font6);

                GetValue(e, "\n", font6);
                GetValue(e, "STC : AACCH1931QST001", font6);
                GetValue(e, "TIN : 29480846364", font6);
                var font = 6;
                GetValue(e, "***Cheque & Card payments are subject to realization", font);

                GetValue(e, "\n", 8);
                GetValue(e, "www.facebook.com/rafisir", font6);
                GetValue(e, "www.handwritingindia.com", font6);

                GetValue(e, "\n", 20);
               // GetValue(e, "                  --------------------------------------", font6);
               // GetValue(e, "                                Signature", font6);
                GetValue(e, "\n", 4);
                GetValue(e, "                            "+ copyName, font6);
            }

            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }

        private static void GetValue(PrintPageEventArgs e, string thisIsATest, int font)
        {
            const int xaxis = 20;
            var printFont = new Font(_fontFamily, font);
            e.Graphics.DrawString(thisIsATest, printFont, Brushes.Black, xaxis, _yPos);

            _yPos = _yPos + font+8;
        }

        private static string   GetHeaderToPrint(string input)
        {
            return $"---{input}---";
        }
   
        public static void PrintDocAsDocx(NewEntryViewModel newEntryViewModel)
        {
            var appPath = AppDomain.CurrentDomain.BaseDirectory;
            var fileNameFull = Path.Combine(appPath, "printTemplate.docx");
            _newEntryViewModel = newEntryViewModel;

            var doc = DocX.Load(fileNameFull);
            doc.ReplaceText("{name}",newEntryViewModel.Name);
            doc.ReplaceText("{phone}",newEntryViewModel.Phone);
            doc.ReplaceText("{billno}",newEntryViewModel.BillNo);
            doc.ReplaceText("{timestamp}",newEntryViewModel.TimeStamp.ToString("dd-MM-yy hh:mm tt"));
            doc.ReplaceText("{balance}",newEntryViewModel.Balance);
            doc.ReplaceText("{course}",newEntryViewModel.Course.CourseName);
            doc.ReplaceText("{coursefee}", "₹ " + newEntryViewModel.Course.Fee.ToString("##.###"));
            doc.ReplaceText("{centername}",newEntryViewModel.Center.CenterName);

            var courseAmount = _newEntryViewModel.AmountPaid / _reverseTax;
            doc.ReplaceText("{towardscouse}", "₹ " + courseAmount.ToString("##.###"));
            doc.ReplaceText("{towardstax}", "₹ " + (courseAmount * (_tax / 100)).ToString("##.###"));
            doc.ReplaceText("{totalamount}", "₹ " + _newEntryViewModel.AmountPaid.ToString("##.###"));
            doc.ReplaceText("{taxpc}", _tax.ToString("##.###")+"%");
            var filenamenew = fileNameFull.Replace("printTemplate", DateTime.Now.Ticks.ToString());
            doc.SaveAs(filenamenew);

            Application wordApp = new Application();

            var doc2 = GetMsDocument(filenamenew, wordApp);

            PrintDialog pdi = new PrintDialog { PrinterSettings = new PrinterSettings() };

            pdi.ShowDialog();

            doc2.Application.ActivePrinter = pdi.PrinterSettings.PrinterName;
            try
            {
                doc2.Application.PrintOut();
            }
            catch (Exception)
            {
                return;
            }
            wordApp = null;
        }

        private static Document GetMsDocument(string filenamenew, Application wordApp)
        {
            object fileName = filenamenew;
            object confirmConversions = Type.Missing;
            object readOnly = true;
            object addToRecentFiles = Type.Missing;
            object passwordDoc = Type.Missing;
            object passwordTemplate = Type.Missing;
            object revert = Type.Missing;
            object writepwdoc = Type.Missing;
            object writepwTemplate = Type.Missing;
            object format = Type.Missing;
            object encoding = Type.Missing;
            object visible = Type.Missing;
            object openRepair = Type.Missing;
            object docDirection = Type.Missing;
            object notEncoding = Type.Missing;
            object xmlTransform = Type.Missing;

            Document doc2 = wordApp.Documents.Open(
                ref fileName, ref confirmConversions, ref readOnly, ref addToRecentFiles,
                ref passwordDoc, ref passwordTemplate, ref revert, ref writepwdoc,
                ref writepwTemplate, ref format, ref encoding, ref visible, ref openRepair,
                ref docDirection, ref notEncoding, ref xmlTransform);
            return doc2;
        }
    }
}

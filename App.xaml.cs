using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RFQ_Compiler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // do something with the file
            MainWindow mainWindow = new MainWindow();

            if (e.Args.Length > 0)
            {
                string filePath = e.Args[0];
                if (System.IO.File.Exists(filePath))
                {
                    string ext = System.IO.Path.GetExtension(filePath).ToLower();

                    switch (ext)
                    {
                        case ".rfq":
                            mainWindow.viewModel.Quote.LoadFile(filePath);
                            break;
                        case ".sldasm":
                            mainWindow.viewModel.Quote = new Quote(filePath);
                            break;
                        default:
                            MessageBox.Show($"Unsupported file type: {ext}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                    }
                }
            }

            mainWindow.Show();
        }
    }
}
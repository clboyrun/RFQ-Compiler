using Microsoft.Win32;
using System.Windows;

namespace RFQ_Compiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel viewModel = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void SelectQuoteFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "SolidWorks Assemblies|*.sldasm|SolidWorks Parts|*.sldprt|SolidWorks Drawings|*.slddrw",
                Multiselect = false
            };

            switch (openFileDialog.ShowDialog())
            {
                case true:
                    string file = openFileDialog.FileName;

                    viewModel.Quote.RootFile.OpenFile(file, true, true);
                    if (PdmExtension.PdmVault != null)
                    {
                        viewModel.Quote.RootFile.PdmRefreshCommand.Execute(file);
                        viewModel.Quote.RootFile.PdmBomRefreshCommand.Execute(file);
                    }
                    else
                    {
                        viewModel.Quote.RootFile.SwRefreshCommand.Execute(file);
                    }
                    break;
            }
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All Files|*.*",
                Multiselect = true
            };

            switch (openFileDialog.ShowDialog())
            {
                case true:
                    foreach (var file in openFileDialog.FileNames)
                    {
                        viewModel.Quote.RootFile.BomFiles.Add(new PackageFile(file));
                    }
                    break;
            }
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "RFQ Files|*.rfq",
                Multiselect = false
            };

            switch (openFileDialog.ShowDialog())
            {
                case true:
                    viewModel.Quote.LoadFile(openFileDialog.FileName);
                    break;
            }
        }

        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            viewModel.NewQuote();
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Quote.SaveFile(viewModel.QuoteFilePath);
        }

        private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "RFQ Files|*.rfq"
            };

            switch (saveFileDialog.ShowDialog())
            {
                case true:
                    viewModel.QuoteFilePath = saveFileDialog.FileName;
                    viewModel.Quote.SaveFile(viewModel.QuoteFilePath);
                    break;
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var wih = new System.Windows.Interop.WindowInteropHelper(this);
            var hWnd = wih.Handle;
            PdmExtension.SetWindowHandle(hWnd);
        }

        private void MenuSettingsLocal_Click(object sender, RoutedEventArgs e)
        {
            UserSettings userSettings = new UserSettings();
            userSettings.ShowDialog();
        }

        private void MenuSettingsPdm_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
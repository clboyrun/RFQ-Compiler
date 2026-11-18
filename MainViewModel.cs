using EPDM.Interop.epdm;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;

namespace RFQ_Compiler
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Private Properties

        private readonly string _title = "RFQ Compiler";
        private string statusBarMessage = "";

        #endregion

        #region Public Properties

        public string Title
        {
            get
            {
                if (!string.IsNullOrEmpty(RfqNumber))
                    return $"{_title} - {RfqNumber}";
                return _title;
            }
        }

        public string[] RecentFiles { get { return new string[] { }; } }

        public string QuoteFilePath { get; set; }
        public Quote Quote { get; set; }
        public string RfqNumber
        {
            get => Quote?.RfqNumber;
            set
            {
                if (Quote != null)
                Quote.RfqNumber = value;

                OnPropertyChanged();
                OnPropertyChanged(Title);
            }
        }

        public ICommand ExportCommand { get; }

        public string StatusBarDisplayMessage
        {
            get
            {
                return statusBarMessage;
            }
            set
            {
                statusBarMessage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public MainViewModel()
        {
            Quote = new Quote();
            ExportCommand = new RelayCommand(ExportEvent);
        }

        public void NewQuote()
        {
            QuoteFilePath = string.Empty;
            Quote = new Quote();
            OnPropertyChanged(nameof(Quote));
            OnPropertyChanged(nameof(Title));
        }

        public void ExportEvent()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Zip Files|*.zip",
                RestoreDirectory = true,
                FileName = Quote.RfqNumber
            };

            switch (saveFileDialog.ShowDialog())
            {
                case true:
                    Quote.Export(saveFileDialog.FileName);
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RFQ_Compiler
{
    public class Quote : INotifyPropertyChanged
    {
        #region Private Properties

        private string _rfqNumber;
        private DateTime _creationDate;
        private QuoteFile _rootFile;
        private List<PackageFile> _packages;

        #endregion

        #region Public Properties

        public string RfqNumber
        {
            get => _rfqNumber;
            set
            {
                _rfqNumber = value;
                OnPropertyChanged();
            }
        }

        public DateTime CreationDate
        {
            get => _creationDate;
            set
            {
                _creationDate = value;
                OnPropertyChanged();
            }
        }

        public QuoteFile RootFile
        {
            get => _rootFile;
            set
            {
                _rootFile = value;
                OnPropertyChanged();
            }
        }

        public List<PackageFile> PackageFiles
        {
            get => _packages;
            set
            {
                _packages = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
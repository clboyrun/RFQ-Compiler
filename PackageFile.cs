using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RFQ_Compiler
{
    public class PackageFile : INotifyPropertyChanged
    {
        #region Private Properties

        private string _filePath;
        private string _fileName;
        private int _documentId;
        private int _version;
        private string _revision;

        #endregion

        #region Public Properties

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }
        public int DocumentId
        {
            get { return _documentId; }
            set
            {
                _documentId = value;
                OnPropertyChanged();
            }
        }
        public int Version
        {
            get { return _version; }
            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }
        public string Revision
        {
            get { return _revision; }
            set
            {
                _revision = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
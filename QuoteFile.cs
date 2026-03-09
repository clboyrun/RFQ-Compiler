using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFQ_Compiler
{
    public class QuoteFile : PackageFile
    {
        #region Private Properties

        private List<PackageFile> bomFiles;

        #endregion

        #region Public Properties

        public List<PackageFile> BomFiles
        {
            get => bomFiles;
            set
            {
                bomFiles = value;
                OnPropertyChanged(); // If you want to implement INotifyPropertyChanged
            }
        }

        #endregion
    }
}

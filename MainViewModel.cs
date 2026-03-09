using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFQ_Compiler
{
    public class MainViewModel
    {
        #region Private Properties

        private readonly string _title = "RFQ Compiler";

        #endregion

        #region Public Properties

        public string FileName { get; set; }
        
        public string Title { get { return _title; } }

        public string[] RecentFiles { get { return new string[] { }; } }

        public Quote Quote { get; set; }

        #endregion
    }
}

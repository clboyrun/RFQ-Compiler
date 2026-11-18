using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RFQ_Compiler
{
    public class Quote : INotifyPropertyChanged
    {
        #region Private Properties

        private string _rfqNumber;
        private DateTime _creationDate;
        private QuoteFile _rootFile;

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

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool SaveFile(string path)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(path))
            {
                using (JsonTextWriter writer = new JsonTextWriter(sw))
                {
                    try
                    {
                        jsonSerializer.Serialize(writer, this);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return true;
        }

        public bool LoadFile(string path)
        {
            Quote quote;
            JsonSerializer jsonSerializer = new JsonSerializer();
            using (StreamReader sr = new StreamReader(path))
            {
                using (JsonTextReader reader = new JsonTextReader(sr))
                {
                    try
                    {
                        quote = jsonSerializer.Deserialize<Quote>(reader);
                        this.RfqNumber = quote.RfqNumber;
                        this.CreationDate = quote.CreationDate;
                        this.RootFile = quote.RootFile;
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Exports all files flagged to be included to a zip file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Export(string path)
        {
            var filesToZip = RootFile.BomFiles.Where(x => x.IsExport).ToList();

            using (var archive = ZipFile.Open(path, ZipArchiveMode.Create))
            {
                foreach (var file in filesToZip)
                {
                    if (file.IsLocal())
                    {
                        archive.CreateEntryFromFile(
                            file.FilePath,
                            file.Name,
                            file.CompressionLevel
                        );
                    }
                }
            }

            return true;
        }

        public Quote()
        {
            _rfqNumber = string.Empty;
            _creationDate = DateTime.Now;
            _rootFile = new QuoteFile();
        }

        public Quote(string path)
        {
            _rfqNumber = Path.GetFileNameWithoutExtension(path).ToLower();
            _creationDate = DateTime.Now;
            _rootFile = new QuoteFile();
            _rootFile.OpenFile(path);

            if (PdmExtension.PdmVault != null)
            {
                Task a = _rootFile.PdmRefreshCommand.ExecuteAsync(this);;
                Task b = _rootFile.PdmBomRefreshCommand.ExecuteAsync(this);
            }
            else
            {
                _rootFile.SwRefreshCommand.Execute(this);
            }
        }
    }
}
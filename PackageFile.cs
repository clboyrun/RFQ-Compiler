using EPDM.Interop.epdm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RFQ_Compiler
{
    public class PackageFile : INotifyPropertyChanged, IComparable<PackageFile>, IEquatable<PackageFile>
    {
        #region Private Properties

        protected string _filePath;
        private string _description;
        private string _fileName;
        private string _extension;
        private int _documentId;
        private int _version;
        private string _revision;
        private bool _isBom;
        private bool _isExport;

        #endregion

        #region Public Properties

        public string FilePath
        {
            get
            {
                if (Version == 0)
                    return _filePath;

                return $"{_filePath} (Version: {Version})";
            }
            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                {
                    _fileName = Path.GetFileName(_filePath);
                    OnPropertyChanged();
                }
                return _fileName;
            }
        }
        [JsonIgnore]
        public string Extension
        {
            get
            {
                if (string.IsNullOrEmpty(_extension))
                {
                    _extension = Path.GetExtension(_filePath);
                    OnPropertyChanged();
                }
                return _extension;
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
                OnPropertyChanged(FilePath);
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
        public bool IsBom
        {
            get { return _isBom; }
            set
            {
                _isBom = value;
                OnPropertyChanged();
            }
        }
        public bool IsExport
        {
            get { return _isExport; }
            set
            {
                _isExport = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IncludeCommandVisibility));
                OnPropertyChanged(nameof(RemoveCommandVisibility));
                OnPropertyChanged(nameof(IconExportType));
            }
        }
        [JsonIgnore]
        public System.Windows.Media.ImageSource IconFileType
        {
            get
            {
                System.Windows.Media.ImageSource toReturn = null;

                string ext = System.IO.Path.GetExtension(FilePath).ToLowerInvariant().TrimStart('.');
                Uri extPath = new Uri($"pack://application:,,,/Icons/Exts/{ext}.ico");

                if (toReturn == null)
                    try { toReturn = new BitmapImage(extPath); } catch (Exception) { }

                if (toReturn == null)
                    toReturn = FileIconProvider.GetIcon(FilePath, ext);

                return toReturn;
            }
        }
        [JsonIgnore]
        public string IconBomType
        {
            get
            {
                return "pack://application:,,,/Icons/BOM.png";
            }
        }
        [JsonIgnore]
        public string IconExportType
        {
            get
            {
                return "pack://application:,,,/Icons/Export_Color.png";

                if (_isExport) return "pack://application:,,,/Icons/Export_Color.png";
                else return "pack://application:,,,/Icons/Export_Grey.png";
            }
        }
        [JsonIgnore]
        public string IncludeCommandVisibility
        {
            get
            {
                if (IsExport)
                    return "Collapsed";
                else
                    return "Visible";
            }
        }
        [JsonIgnore]
        public string RemoveCommandVisibility
        {
            get
            {
                if (IsExport)
                    return "Visible";
                else
                    return "Collapsed";
            }
        }
        [JsonIgnore]
        public CompressionLevel CompressionLevel
        {
            get
            {
                if (IsExport)
                    return CompressionLevel.Optimal;
                else
                    return CompressionLevel.Fastest;
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        int IComparable<PackageFile>.CompareTo(PackageFile other)
        {
            if (other == null)
                return 0;
            if (ReferenceEquals(this, other))
                return 1;
            return this.FilePath.CompareTo(other.FilePath);
        }

        bool IEquatable<PackageFile>.Equals(PackageFile other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (this.FilePath.Equals(other.FilePath, StringComparison.InvariantCultureIgnoreCase))
            {
                if (this.Revision.Equals(other.Revision, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Commands

        [JsonIgnore]
        public RelayCommand OpenCommand { get; }
        [JsonIgnore]
        public RelayCommand InvertCommand { get; }
        [JsonIgnore]
        public RelayCommand SelectRevisionCommand { get; }
        [JsonIgnore]
        public RelayCommand PdmRefreshCommand { get; }
        [JsonIgnore]
        public RelayCommand SwRefreshCommand { get; }

        private void CommandOpen()
        {
            if (File.Exists(FilePath))
                Process.Start(FilePath);
            else
                MessageBox.Show("File not found: " + FilePath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CommandInvert()
        {
            IsExport = !IsExport;
        }

        private void CommandSelectRevision()
        {
            // TODO Implement the logic to select a specific version of the file
            CommandPdmRefresh();
        }

        private void CommandPdmRefresh()
        {
            IEdmFile5 file = PdmExtension.GetFile(_filePath, out IEdmFolder5 folder);

            IEdmEnumeratorVersion5 versionEnumerator = (IEdmEnumeratorVersion5)file;

            IEdmVersion5 ver = default(IEdmVersion5);
            if (Version > 0)
            {
                ver = versionEnumerator.GetVersion(Version);
            }
            else
            {
                ver = versionEnumerator.GetVersion(file.CurrentVersion);
            }
            
            ver.GetFileCopy(PdmExtension.WindowHandleId, folder.ID, (int)EdmGetFlag.EdmGet_MakeReadOnly | (int)EdmGetFlag.EdmGet_Refs);

            IEdmEnumeratorVariable5 varEnumeration = file.GetEnumeratorVariable();
            varEnumeration.GetVar("Description", "", out object desc);
            string revision = file.CurrentRevision;

            Revision = revision;
            if (desc == null) desc = string.Empty;
            Description = desc.ToString();
        }

        private void CommandSwRefresh()
        {
            // TODO Implement the logic to refresh the file information from SolidWorks
        }

        #endregion

        public PackageFile(string path = "", bool isBom = false, bool include = false)
        {
            _filePath = path;
            _documentId = 0;
            _version = 0;
            _revision = string.Empty;
            _isBom = isBom;
            _isExport = include;
            if (PdmExtension.IsVaultPath(path, out string vault)) PdmExtension.SetVaultName(vault);

            OpenCommand = new RelayCommand(CommandOpen);
            InvertCommand = new RelayCommand(CommandInvert);
            SelectRevisionCommand = new RelayCommand(CommandSelectRevision);
            PdmRefreshCommand = new RelayCommand(CommandPdmRefresh);
            SwRefreshCommand = new RelayCommand(CommandSwRefresh);
        }

        public bool OpenFile(string path, bool isBom = false, bool include = false)
        {
            try
            {
                FilePath = path;
                DocumentId = 0;
                Version = 0;
                Revision = "";
                _isBom = isBom;
                _isExport = include;
                if (PdmExtension.IsVaultPath(path, out string vault)) PdmExtension.SetVaultName(vault);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<string> LoadReferences()
        {
            HashSet<string> references = new HashSet<string>(LoadUserDefinedReferences(), StringComparer.InvariantCultureIgnoreCase);
            if (PdmExtension.PdmVault != null)
                references.AddRange(LoadPdmReferences());
            if (SwExtension.IsSolidWorksFile(FilePath))
                references.AddRange(LoadSwReferences());

            return new string[] { };
        }

        private IEnumerable<string> LoadUserDefinedReferences()
        {
            string localFolder = new FileInfo(FilePath).Directory.FullName;
            string rootName = Path.GetFileNameWithoutExtension(FilePath);
            List<string> refsToReturn = new List<string>();

            string pdfPath = $"{localFolder}\\Released PDF\\{rootName}.pdf";
            string stpPath = $"{localFolder}\\Released STEP\\{rootName}_Rev {Revision}.step";
            string dxfPath = $"{localFolder}\\Released DXF\\{rootName}_Rev {Revision}.dxf";

            if (File.Exists(pdfPath)) refsToReturn.Add(pdfPath);
            if (File.Exists(stpPath)) refsToReturn.Add(stpPath);
            if (File.Exists(dxfPath)) refsToReturn.Add(dxfPath);

            return refsToReturn;
        }

        private IEnumerable<string> LoadSwReferences()
        {


            return new string[] { };
        }

        private IEnumerable<string> LoadPdmReferences()
        {
            IEdmFile5 file = PdmExtension.GetFile(_filePath, out IEdmFolder5 folder);
            int ver = Version > 0 ? Version : file.CurrentVersion;
            IEnumerable<IEdmFile5> referenceList = file.GetUserDefinedReferences(folder.ID, ver);
            List<string> refNames = new List<string>();
            foreach (IEdmFile5 f in referenceList)
            {
                IEdmPos5 pos = f.GetFirstFolderPosition();
                while (!pos.IsNull)
                {
                    IEdmFolder5 refFolder = f.GetNextFolder(pos);
                    string path = f.GetLocalPath(refFolder.ID);
                    refNames.Add(path);
                }
            }

            return refNames;
        }

        public bool IsLocal()
        {
            //TODO Cache PDM version if appropriate

            return File.Exists(FilePath);
        }
    }
}
using EPDM.Interop.epdm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace RFQ_Compiler
{
    public class QuoteFile : PackageFile
    {
        #region Private Properties

        private List<PackageFile> bomFiles;
        private string vaultName;

        #endregion

        #region Public Properties

        public List<PackageFile> BomFiles
        {
            get => bomFiles;
            set
            {
                bomFiles = value;
                OnPropertyChanged();
            }
        }

        public string VaultName
        {
            get => vaultName;
            set
            {
                vaultName = value;
                OnPropertyChanged();

                PdmExtension.SetVaultName(value);
            }
        }

        #endregion

        public QuoteFile()
        {
            bomFiles = new List<PackageFile>();
            PdmBomRefreshCommand = new RelayCommand(CommandPdmBomRefresh);
        }

        [JsonIgnore]
        public RelayCommand PdmBomRefreshCommand { get; }

        private void CommandPdmBomRefresh()
        {
            IEdmVariableMgr5 mgr = (IEdmVariableMgr5)PdmExtension.PdmVault;
            IEdmVariable5 revVariable = mgr.GetVariable("Revision");
                IEdmFile5 file = PdmExtension.GetFile(_filePath, out IEdmFolder5 folder);
            IEdmEnumeratorVersion5 versionEnumerator = (IEdmEnumeratorVersion5)file;
            IEdmVersion5 ver = default(IEdmVersion5);
            if (Version > 0)
            { ver = versionEnumerator.GetVersion(Version); }
            else
            { ver = versionEnumerator.GetVersion(file.CurrentVersion); }

            // Finish processing bom
            IEdmFile7 edmFile7 = (IEdmFile7)file;
            string[] boms = PdmExtension.GetBomNames();

            foreach (var bominfo in boms)
            {
                EdmBomView bomView = edmFile7.GetComputedBOM(bominfo, ver.VersionNo, string.Empty, (int)EdmBomFlag.EdmBf_ShowSelected);
                bomView.GetRows(out object[] bomRows);
                foreach (EdmBomCell cell in bomRows)
                {
                    string childPath = cell.GetPathName();
                    PackageFile childRef = new PackageFile(childPath, true, false);
                    BomFiles.Add(childRef);

                    IEnumerable<string> refs = childRef.LoadReferences();
                    foreach(string r in refs)
                    {
                        IEdmFile5 f = PdmExtension.GetFile(r, out IEdmFolder5 fldr);
                        bool include = false;
                        switch(f.CurrentState.Name)
                        {
                            case "Released PDF":
                                include = Path.GetExtension(r).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
                                break;
                            case "Released DXF":
                                include = Path.GetExtension(r).Equals(".dxf", StringComparison.OrdinalIgnoreCase);
                                break;
                            case "Released STEP":
                                include = Path.GetExtension(r).Equals(".step", StringComparison.OrdinalIgnoreCase);
                                break;
                        }
                        PackageFile child = new PackageFile(r, false, include);
                        BomFiles.Add(child);
                    }
                }
            }

            OnPropertyChanged(nameof(BomFiles));
        }

    }
}

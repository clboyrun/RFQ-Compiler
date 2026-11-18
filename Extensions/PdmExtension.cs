using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFQ_Compiler
{
    public static class PdmExtension
    {
        private static EdmVault5 _vault = null;
        private static string _vaultName = null;
        private static int _windowHandle = 0;

        public static EdmVault5 PdmVault
        {
            get
            {
                if (_vault == null)
                {
                    _vault = new EdmVault5();
                    if (_vaultName != null)
                    { _vault.LoginAuto(_vaultName, _windowHandle); }
                }
                return _vault;
            }
        }

        public static int WindowHandleId
        {
            get { return _windowHandle; } 
        }

        public static void SetVaultName(string vaultName)
        {
            if (_vaultName == null || !_vaultName.Equals(vaultName, StringComparison.InvariantCultureIgnoreCase))
            {
                _vault = null;
                _vaultName = vaultName;
            }
        }

        public static void SetWindowHandle(IntPtr windowHandle)
        {
            _windowHandle = windowHandle.ToInt32();
        }

        public static IEdmFile5 GetFile(string path, out IEdmFolder5 folder)
        {
            folder = null;

            if (PdmVault == null)
                return null;

            return PdmVault.GetFileFromPath(path, out folder);
        }

        public static bool IsVaultPath(string path)
        {
            if (PdmVault == null)
                return false;
            PdmVault.GetVaultNameFromPath(path);
            return true;
        }

        public static bool IsVaultPath(string path, out string vaultName)
        {
            vaultName = string.Empty;
            if (string.IsNullOrEmpty(path))
                return false;
            if (PdmVault == null)
                return false;
            vaultName = PdmVault.GetVaultNameFromPath(path);
            return true;
        }

        private static List<string> bomNames = null;
        public static string[] GetBomNames()
        {
            if (bomNames == null)
            {
                IEdmVault11 vault2 = (IEdmVault11)PdmVault;
                IEdmBomMgr2 bomMgr = (IEdmBomMgr2)PdmVault.CreateUtility(EdmUtility.EdmUtil_BomMgr);
                int userId = vault2.GetLoggedInWindowsUserID(vault2.Name);
                bomMgr.GetBomLayouts2(out EdmBomLayout2[] ppoRetLayouts);
                bomNames = new List<string>();
                for (int i = 0; i < ppoRetLayouts.Length; i++)
                {
                    if (bomMgr.CanSeeBomLayout(ppoRetLayouts[i].mlLayoutID, userId))
                        bomNames.Add(ppoRetLayouts[i].mbsLayoutName);
                }
            }

            return bomNames.ToArray();
        }

        public static IEnumerable<IEdmFile5> GetUserDefinedReferences(this IEdmFile5 file, int folderId, int version)
        {
            IEdmReference7 reference = (IEdmReference7)file.GetReferenceTree(folderId, version);
            string projectName = string.Empty;
            IEdmPos5 children = reference.GetFirstChildPosition2(ref projectName, true, true, (int)EdmRefFlags.EdmRef_File, version);

            List<IEdmFile5> references = new List<IEdmFile5>();
            while (children.IsNull == false)
            {
                reference = (IEdmReference7)reference.GetNextChild(children);
                references.Add(reference.File);
            }

            return references;
        }

        public static string Replace(this string file, Dictionary<string, string> toReplace)
        {
            string output = file;
            foreach (var kvp in toReplace)
            {
                output = output.Replace(kvp.Key, kvp.Value);
            }
            return output;
        }

        public static void AddRange(this HashSet<string> strings, IEnumerable<string> list)
        {
            foreach(string item in list)
                strings.Add(item);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFQ_Compiler
{
    internal class PdmSettings
    {
        public static HashSet<string> UserDefinedReferenceFilePaths = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        //private static Dictionary<string, string> PathReplacements = new Dictionary<string, string>() { { "[VaultRoot]", "" } };
        //private static Dictionary<string, RelayCommand> PathFunctionReplacements = new Dictionary<string, RelayCommand>() { { "[VaultRoot]", "" } };

        public static List<string> GetUserDefinedReferenceFilePaths(string parentFile)
        {
            foreach (string path in UserDefinedReferenceFilePaths)
            {
                string destinationFolderPath = path.Replace("", "");
            }

            return null;
        }
    }
}

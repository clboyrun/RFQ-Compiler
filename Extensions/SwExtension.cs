using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFQ_Compiler
{
    public static class SwExtension
    {
        public static bool IsSolidWorksFile(string file)
        {
            string ext = Path.GetExtension(file).ToLowerInvariant();

            switch (ext)
            {
                case ".sldprt":
                case ".sldasm":
                case ".slddrw":
                    return true;
            }

            return false;
        }
    }
}

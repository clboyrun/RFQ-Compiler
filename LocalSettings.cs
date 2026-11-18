using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RFQ_Compiler
{
    internal class LocalSettings
    {
        private bool? _isRegistered = null;
        private string InstallationPath = @"C:\Program Files\LynxForged Tools\RFQ Compiler\RFQ Compiler.exe";

        private RegistryKey _registryAdmin
        {
            get
            {
                try
                {
                    return RegistryKey
                        .OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Default)
                        .CreateSubKey("SldAssem.Document")
                        .CreateSubKey("shell");
                }
                catch { }
                return null;
            }
        }
        private RegistryKey _registryUser
        {
            get
            {
                try
                {
                    return RegistryKey
                        .OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default)
                        .CreateSubKey("Software")
                        .CreateSubKey("Classes")
                        .CreateSubKey("SldAssem.Document")
                        .CreateSubKey("shell");
                }
                catch { }
                return null;
            }
        }
        private static string RegistryKeyName = "GeneratePackage";

        public bool IsRunningElevated = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        public bool IsRegisteredCommand
        {
            get
            {
                if (_isRegistered == null)
                {
                    _isRegistered = _registryUser.GetSubKeyNames().Contains(RegistryKeyName);
                }
                return (bool)_isRegistered;
            }
            set
            {
                _isRegistered = value;

                switch (value)
                {
                    case true:
                        RegisterCommand();
                        break;
                    case false:
                        UnregisterCommand();
                        break;
                }
            }
        }

        public LocalSettings()
        {
        }

        public void RegisterCommand()
        {
            RegisterCommand(_registryUser);
            if (IsRunningElevated)
                RegisterCommand(_registryAdmin);
        }

        public void UnregisterCommand()
        {
            try
            {
                _registryUser.DeleteSubKeyTree(RegistryKeyName, false);
                if (IsRunningElevated)
                    _registryAdmin.DeleteSubKeyTree(RegistryKeyName, false);
            }
            catch { }
        }

        private void RegisterCommand(RegistryKey regKey)
        {
            try
            {
                RegistryKey reg = regKey.CreateSubKey(RegistryKeyName);
                reg.SetValue("", "Generate Package with RFQ Compiler");
                reg = reg.CreateSubKey("command");
                reg.SetValue("", $@"""{InstallationPath}"" ""%1""");
            }
            catch { }
        }
    }
}
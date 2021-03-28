using System;
using System.Reflection;
using System.Runtime.Loader;

namespace GeneratePdfHttpFunction.Configuration
{
    internal class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            try
            {
                return LoadUnmanagedDllFromPath(unmanagedDllName);
            }
            catch (Exception)
            {
                return IntPtr.Zero;
            }

        }
        protected override Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
    }
}

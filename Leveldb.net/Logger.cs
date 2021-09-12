using System.Runtime.InteropServices;

namespace LevelDB
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Log(string msg);

    public class Logger : LevelDBHandle
    {
        public Logger(Log log)
        {
        }

        public static implicit operator Logger(Log log)
        {
            return new Logger(log);
        }

        protected override void FreeUnManagedObjects()
        {
        }
    }
}

using System;
using System.Runtime.InteropServices;

using LevelDB.NativePointer;

namespace LevelDB
{
    internal class PinnedSafeHandle<T> : SafeHandle
        where T : struct
    {
        private GCHandle pinnedRawData;

        public PinnedSafeHandle(T[] arr)
            : base(default(IntPtr), true)
        {
            pinnedRawData = GCHandle.Alloc(arr, GCHandleType.Pinned);

            // initialize handle last; ensure we only free initialized GCHandles.
            handle = pinnedRawData.AddrOfPinnedObject();
        }

        public Ptr<T> Ptr
        {
            get
            {
                return (Ptr<T>)handle;
            }
        }

        public override bool IsInvalid
        {
            get
            {
                return handle == default(IntPtr);
            }
        }

        protected override bool ReleaseHandle()
        {
            if (handle != default(IntPtr))
            {
                pinnedRawData.Free();
                handle = default(IntPtr);
            }
            return true;
        }
    }
}

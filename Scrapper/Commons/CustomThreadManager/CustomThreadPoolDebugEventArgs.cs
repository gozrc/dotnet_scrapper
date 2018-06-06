using System;

namespace Commons.CustomThreadManager
{
    public delegate void CustomThreadPoolDebugHandler (object sender, CustomThreadPoolDebugEventArgs args);

    public class CustomThreadPoolDebugEventArgs : EventArgs
    {
        public readonly string dump = string.Empty;

        public CustomThreadPoolDebugEventArgs (string dump)
        {
            this.dump = dump;
        }
    }
}

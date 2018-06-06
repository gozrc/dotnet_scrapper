using System;

namespace Commons.CustomThreadManager
{
    public delegate void CustomThreadTaskDebugHandler(object sender, CustomThreadTaskDebugEventArgs args);

    public class CustomThreadTaskDebugEventArgs
    {
        public readonly string dump = string.Empty;

        public CustomThreadTaskDebugEventArgs (string dump)
        {
            this.dump = dump;
        }
    }
}

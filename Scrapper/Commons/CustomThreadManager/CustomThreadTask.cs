using System;
using System.Threading;

namespace Commons.CustomThreadManager
{
    public delegate void FTask (object state);

    public class CustomThreadTask
    {
        public event CustomThreadTaskDebugHandler onCustomThreadTaskDebug;

        FTask  _task  = null;
        string _name  = string.Empty;
        object _state = null;


        public CustomThreadTask (FTask task, object state)
        {
            _task  = task;
            _state = state;
        }

        public CustomThreadTask (FTask task, object state, string name) 
            : this(task, state)
        {
            _name  = name;
        }

        public CustomThreadTask (FTask task, object state, string name, CustomThreadTaskDebugHandler onDebug)
            : this(task, state, name)
        {
            this.onCustomThreadTaskDebug += onDebug;
        }


        public void run ()
        {
            try
            {
                debug ("Task init (" + _name + ")");

                _task (_state);

                debug ("Task end (" + _name + ")");
            }
            catch (ThreadAbortException)
            {
                debug ("Task (" + _name + ") ABORTED!");
            }
        }

        public string name
        {
            get { return _name; }
        }


        public override string ToString ()
        {
            return this.name;
        }


        void debug (string info)
        {
            if (null != onCustomThreadTaskDebug)
            {
                onCustomThreadTaskDebug(
                    this,
                    new CustomThreadTaskDebugEventArgs(info)
                );
            }
        }
    }
}

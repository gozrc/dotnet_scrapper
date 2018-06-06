using System;
using System.Collections.Generic;
using System.Threading;

namespace Commons.CustomThreadManager
{
    public class CustomThreadPool : IDisposable
    {
        public event CustomThreadPoolDebugHandler onCustomThreadPoolDebug;

        int                      _maxThreadsInExecution = 0;
        Queue<CustomThreadTask>  _queueTasks            = null;
        object                   _lockQueue             = null;
        Thread                   _mainThread            = null;
        bool                     _running               = false;
        Thread[]                 _customThreadPool      = null;
        int                      _stopTimeout           = 0;


        public CustomThreadPool (int maxThreadsInExecution, int stopTimeout, CustomThreadPoolDebugHandler onDebug)
        {
            this.onCustomThreadPoolDebug += onDebug;
            initialize(maxThreadsInExecution, stopTimeout);
        }

        public CustomThreadPool (int maxThreadsInExecution, int stopTimeout)
        {
            initialize(maxThreadsInExecution, stopTimeout);
        }


        void initialize (int maxThreadsInExecution, int stopTimeout)
        {
            _maxThreadsInExecution = maxThreadsInExecution;
            _queueTasks            = new Queue<CustomThreadTask>();
            _lockQueue             = new object();
            _stopTimeout           = stopTimeout;

            _mainThread = new Thread(
                new ThreadStart(this.mainThreadFunction));

            _mainThread.IsBackground = false;
            _mainThread.Name         = "CustomThreadPool.MainThread";

            _running = true;

            _customThreadPool = new Thread[_maxThreadsInExecution];

            _mainThread.Start();
        }


        public void queueTask (CustomThreadTask task)
        {
            lock (_lockQueue)
            {
                _queueTasks.Enqueue(task);
                debug ("Task Added: " + task.name);
            }
        }

        public void terminate ()
        {
            _running = false;

            while (_mainThread.IsAlive)
                Thread.Sleep(10);
        }

        public void Dispose ()
        {
            terminate();
        }

        public void joinToAll ()
        {
            debug ("joinToAll init");

            while (_queueTasks.Count > 0)
                Thread.Sleep(1);

            debug ("joinToAll 0 tasks");

            for (int k = 0; k < _customThreadPool.Length; k++)
            {
                try
                {
                    while (null != _customThreadPool[k] && _customThreadPool[k].IsAlive)
                        Thread.Sleep(1);
                }
                catch { }
            }

            debug ("joinToAll end");
        }


        void mainThreadFunction ()
        {
            try
            {
                debug ("Main thread start");

                while (_running)
                {
                    int index = 0;

                    if (getThreadAvailable(ref index))
                    {
                        CustomThreadTask task = null;

                        if (getTaskAvailable(ref task))
                            startTask(index, task);
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                debug ("Main thread exception: " + ex.Message);
            }
            finally
            {
                waitForAll ();

                lock (_lockQueue)
                    _queueTasks.Clear();

                for (int k = 0; k < _customThreadPool.Length; k++)
                    _customThreadPool[k] = null;

                debug ("Main thread stop");
            }
        }

        bool getThreadAvailable (ref int index)
        {
            for (int k = 0; k < _customThreadPool.Length; k++)
            {
                if (null != _customThreadPool[k] && _customThreadPool[k].IsAlive)
                    continue;

                index = k;

                return true;
            }

            return false;
        }

        bool getTaskAvailable (ref CustomThreadTask task)
        {
            lock (_lockQueue)
            {
                if (_queueTasks.Count > 0)
                    task = _queueTasks.Dequeue();
            }

            return (null != task);
        }

        void startTask (int index, CustomThreadTask task)
        {
            _customThreadPool[index] = new Thread(
                new ThreadStart(task.run));

            _customThreadPool[index].Name = task.ToString();

            _customThreadPool[index].Start();
        }

        void waitForAll ()
        {
            debug ("Main thread waitForAll start");

            int init = Environment.TickCount;

            for (int k = 0; k < _customThreadPool.Length; k++)
            {
                if (null != _customThreadPool[k])
                {
                    while (_customThreadPool[k].IsAlive)
                    {
                        if (Environment.TickCount - init > _stopTimeout)
                        {
                            _customThreadPool[k].Abort();
                            debug ("Main thread waitForAll timeout!");
                        }

                        Thread.Sleep(1);
                    }
                }
            }

            debug ("Main thread waitForAll end");
        }

        void debug (string info)
        {
            if (null != onCustomThreadPoolDebug)
            {
                onCustomThreadPoolDebug(
                    this, 
                    new CustomThreadPoolDebugEventArgs(info)
                );
            }
        }
    }
}

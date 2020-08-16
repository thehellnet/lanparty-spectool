using System;
using System.Runtime.CompilerServices;
using System.Threading;
using log4net;

namespace LanPartySpecTool.utility
{
    public abstract class StoppableThread
    {
        public delegate void StartHandler();

        public delegate void StopHandler();

        public event StartHandler OnStart;
        public event StopHandler OnStop;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(StoppableThread));

        private readonly object _sync = new object();

        private Thread _thread;
        private bool _keepRunning;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Start()
        {
            _keepRunning = true;
            _thread = new Thread(Loop);
            _thread.Start();

            OnStart?.Invoke();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Stop()
        {
            StopJob();
            _thread.Join();
            _thread = null;
        }

        public void Join()
        {
            if (_thread == null)
            {
                return;
            }

            try
            {
                _thread.Join();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        protected void StopJob()
        {
            _keepRunning = false;
        }


        private void Loop()
        {
            try
            {
                while (_keepRunning)
                {
                    Job();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            OnStop?.Invoke();
        }

        protected abstract void Job();
    }
}
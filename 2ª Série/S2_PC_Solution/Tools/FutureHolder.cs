using System;
using System.Collections.Generic;
using System.Threading;

namespace Tools
{
    struct NullableType<T>
    {
        private T value;
        private bool hasValue;

        public T Value
        {
            get { return value; }
            set { hasValue = true; this.value = value; }
        }
        public bool HasValue
        {
            get { return hasValue; }
        }
    }
    public class FutureHolder<TVal>
    {
        private NullableType<TVal> _val;
        private Object _mon;

        public FutureHolder()
        {            
            _mon = new Object();            
        }
        public TVal Get()
        {
            return Get(Timeout.Infinite);
        }
        public TVal Get(int timeout)
        {
            lock (_mon)
            {                
                if (_val.HasValue)
                    return _val.Value;
         
                try
                {
                    Monitor.Wait(_mon, timeout);
                    return _val.Value;
                }
                catch (ThreadInterruptedException ex)
                {
                    if (_val.HasValue)
                    {
                        Thread.CurrentThread.Interrupt();
                        return _val.Value;
                    }
                    else
                    {
                        throw ex;
                    }
                }         
            }
        }        
        public void Set(TVal val)
        {
            lock (_mon)
            {
                _val.Value = val;
                Monitor.PulseAll(_mon);
            }
        }
    }
}

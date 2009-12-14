using System;

namespace Tools
{
    internal class CacheRecord<T>
    {
        long            lat; //lat == Last Access Time
        FutureHolder<T> fh;  //fh  == Future Holder

        public CacheRecord()
        {
            lat = DateTimeHelper.CurrentTicks;
            fh  = new FutureHolder<T>();            
        }
        
        public long LastAccessTime
        {
            get { return lat; }
        }

        public T Get()
        {
            lat = DateTimeHelper.CurrentTicks;
            return fh.Get();
        }
        public void Set(T val)
        {
            lat = DateTimeHelper.CurrentTicks;
            fh.Set(val);
        }
    }
}

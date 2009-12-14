using System;

namespace Tools
{
    internal class CacheRecord<T>
    {
        long            lat; //lat == Last Access Time
        FutureHolder<T> fh;  //fh  == Future Holder

        public CacheRecord()
        {
            lat = DateTime.UtcNow.Ticks;
            fh  = new FutureHolder<T>();            
        }
        
        public long LastAccessTime
        {
            get { return lat; }
        }

        public T Get()
        {
            lat = DateTime.UtcNow.Ticks;
            return fh.Get();
        }
        public void Set(T val)
        {
            lat = DateTime.UtcNow.Ticks;
            fh.Set(val);
        }
    }
}

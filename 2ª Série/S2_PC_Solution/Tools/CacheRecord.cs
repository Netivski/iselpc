using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools
{
    public class CacheRecord<TVal>
    {
        private DateTime _lastAccessTime;
        private FutureHolder<TVal> _val;

        public CacheRecord()
        {
            _val = new FutureHolder<TVal>();
            _lastAccessTime = DateTime.Now;
        }
        public DateTime LastAccessTime
        {
            get
            {
                return _lastAccessTime;
            }
        }
        public TVal Get()
        {
            _lastAccessTime = DateTime.Now;
            return _val.Get();
        }
        public void Set(TVal val)
        {
            _val.Set(val);
        }
    }
}

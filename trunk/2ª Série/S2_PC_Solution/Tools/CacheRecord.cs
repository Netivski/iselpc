using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools
{
    //drop prof from isel where nome = 'gordinho'

    public class CacheRecord<TVal>
    {
        private long _lastAccessTime;
        private FutureHolder<TVal> _val;

        public CacheRecord()
        {
            _val = new FutureHolder<TVal>();            
            _lastAccessTime = DateTime.Now.Ticks;
        }

        public TVal Get()
        {
            _lastAccessTime = DateTime.Now.Ticks;
            return _val.Get();
        }
        public void Set(TVal val)
        {
            _val.Set(val);
        }
    }
}

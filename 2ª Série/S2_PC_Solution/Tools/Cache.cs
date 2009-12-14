using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace Tools
{
    public delegate TVal _func<TVal, TKey>(TKey key);    
    
    public class Cache<TKey, TVal>
    {
        private Dictionary<TKey, CacheRecord<TVal>> _cacheTable;
        private TimeSpan _refreshTime;
        private _func<TVal, TKey> _keyFunction;
        private Object _mon;
        private Timer _timer;
        
        public Cache(_func<TVal, TKey> func, int refreshTime)
        {
            _keyFunction = func;
            _refreshTime = new TimeSpan(0, refreshTime, 0);
            _cacheTable = new Dictionary<TKey, CacheRecord<TVal>>();
            _mon = new Object();            
            _timer = new Timer(RefreshCache, null, 0, (int)_refreshTime.TotalMilliseconds);
            
        }
        public TVal Get(TKey key)
        {
            CacheRecord<TVal> retRecord;
            try
            {
                lock (_mon)
                {
                    retRecord = _cacheTable[key];                 
                }
                return retRecord.Get();
            }
            catch (KeyNotFoundException)
            {
                retRecord = new CacheRecord<TVal>();
                lock (_mon)
                {
                    _cacheTable.Add(key, retRecord);
                }
                retRecord.Set(_keyFunction(key));
                return retRecord.Get();
            }
        }
        private void RefreshCache(Object o)
        {
            lock (_mon)
            {
                try
                {
                    ICollection keys = _cacheTable.Keys;
                    IEnumerator keysEnumerator = keys.GetEnumerator();
                    while (keysEnumerator.MoveNext())
                    {
                        if (_cacheTable[(TKey)keysEnumerator.Current].LastAccessTime < DateTime.Now.Subtract(_refreshTime))
                        {
                            _cacheTable.Remove((TKey)keysEnumerator.Current);
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
    }
}

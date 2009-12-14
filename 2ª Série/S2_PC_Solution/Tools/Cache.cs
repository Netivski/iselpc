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
        private int _cleanCounter; //just to debug
        
        public Cache(_func<TVal, TKey> func, int refreshTime)
        {
            _keyFunction = func;
            //_refreshTime = new TimeSpan(0, refreshTime, 0);
            _refreshTime = new TimeSpan(0, 0, refreshTime);//just to debug
            _cacheTable = new Dictionary<TKey, CacheRecord<TVal>>();
            _mon = new Object();            
            _timer = new Timer(RefreshCache, null, 0, (int)_refreshTime.TotalMilliseconds);
            _cleanCounter = 0; //just to debug
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
                    try
                    {
                        //Entre a Exception e a entrada aqui, abriu-se a janela!!!
                        //Alguém já pode ter inserido a mesma chave
                        _cacheTable.Add(key, retRecord);
                    }
                    catch (ArgumentException)
                    {
                        retRecord = _cacheTable[key];
                    }
                }
                retRecord.Set(_keyFunction(key));
                return retRecord.Get();
            }
        }
        private void RefreshCache(Object o)
        {
            lock (_mon)
            {
                Console.WriteLine("A iniciar a limpeza N " + ++_cleanCounter);
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
                    Console.WriteLine("Limpeza N " + _cleanCounter + " efectuada sem interrupções");
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Limpeza N " + _cleanCounter + " interrompida");
                }
            }            
        }
    }
}

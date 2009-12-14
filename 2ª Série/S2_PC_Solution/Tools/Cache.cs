using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;

namespace Tools
{       
    public class Cache<K, V>
    {       
        public delegate V SlaveMethod(K key);

        SortedList<K, CacheRecord<V>> cls; // cls == Cache Local Store, Thread Safety
        SlaveMethod sm;                    //  sm == Slave Method
        long        rlt;                   // rlt == Record Life Time
        Object      monitor;        
        Timer       daemon;

        public Cache(SlaveMethod sMethod, long recordLifeTimeMilliseconds)
        {                        
            cls     = new SortedList<K, CacheRecord<V>>();
            sm      = sMethod;            
            rlt     = recordLifeTimeMilliseconds;
            monitor = new Object();

            daemon = new Timer(PurgeCache, null, 0, rlt);
        }
        public V Get(K key)
        {
            CacheRecord<V> retRecord;
            try
            {
                return cls[key].Get(); // Thread Safety
            }
            catch (KeyNotFoundException)
            {
                retRecord = new CacheRecord<V>();
                try
                {
                    //Entre a Exception e a entrada aqui, abriu-se a janela!!! Alguém já pode ter inserido a mesma chave
                    cls.Add(key, retRecord);
                    retRecord.Set(sm(key));
                }
                catch (ArgumentException)
                {
                    retRecord = cls[key];
                }

                return retRecord.Get();
            }
        }

        void PurgeCache(Object o)
        {
            lock (monitor)
            {
                long lt = DateTimeHelper.CurrentTicks + rlt; //lt == Life Time

                cls.Where(p => p.Value.LastAccessTime < lt).Select(p => cls.Remove(p.Key));
            }            
        }
    }
}

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
        const int RECORD_LIFETIME = 500;
        
        public delegate V SlaveMethod(K key);

        SortedList<K, CacheRecord<V>> cls; // cls == Cache Local Store, Thread Safety
        SlaveMethod sm;                    //  sm == Slave Method
        Object monitor;        
        Timer daemon;

        public Cache( SlaveMethod sMethod )
        {            
            cls     = new SortedList<K, CacheRecord<V>>();
            sm      = sMethod;            
            monitor = new Object();

            daemon = new Timer(PurgeCache, null, 0, RECORD_LIFETIME);
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
                lock (monitor)
                {
                    try
                    {
                        //Entre a Exception e a entrada aqui, abriu-se a janela!!!
                        //Alguém já pode ter inserido a mesma chave
                        cls.Add(key, retRecord);
                        retRecord.Set(sm(key));                        
                    }
                    catch (ArgumentException)
                    {
                        retRecord = cls[key];
                    }
                }

                return retRecord.Get();
            }
        }

        void PurgeCache(Object o)
        {
            lock (monitor)
            {
                long rlt = DateTimeHelper.CurrentTicks + RECORD_LIFETIME; //rlt == Record Life Time

                cls.Where(p => p.Value.LastAccessTime < rlt).Select(p => cls.Remove(p.Key));
            }            
        }
    }
}

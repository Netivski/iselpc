using System;
using System.Collections.Generic;
using System.Threading;

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

            daemon = new Timer(PurgeCache, null, rlt, rlt);
        }        

        public V Get(K key)
        {
            #region - Nota 
            //Com esta implementação tenta-se fugir ao estrangulamento na leitura da cls
            //Utilização do lock para escrita.
            #endregion

            CacheRecord<V> record = null;
            bool setValue         = false;

            try 
            {
                record = cls[key];
            }
            catch (KeyNotFoundException)
            {
                try
                {
                    lock (monitor)
                    {
                        record = new CacheRecord<V>();
                        setValue = true;
                        cls.Add(key, record);
                    }
                }
                catch (ArgumentException) //An element with the same key already exists
                {
                    setValue = false;
                    record = cls[key];
                }
            }


            if (setValue)
            {
                record.Set(sm(key));
            }

            return record.Get();
        }

        void PurgeCache(Object o)
        {
            lock (monitor)
            {
                long lt = DateTimeHelper.CurrentTicks - rlt; //lt == Life Time

                //001 - Collect Keys Entries
                LinkedList<K> keys = new LinkedList<K>();
                foreach (KeyValuePair<K, CacheRecord<V>> keyPair in cls) if (keyPair.Value != null && keyPair.Value.LastAccessTime < lt) keys.AddLast(keyPair.Key);

                //001 - Remove Keys Entries
                LinkedList<K>.Enumerator keysEnum = keys.GetEnumerator();
                while (keysEnum.MoveNext()) cls.Remove(keysEnum.Current);                
            }
        }

    }
}

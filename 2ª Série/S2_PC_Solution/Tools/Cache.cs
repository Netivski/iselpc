using System;
using System.Collections;
using System.Collections.Generic;

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

            daemon = new Timer(PurgeCache, null, rlt, rlt);
        }

        //public V Get(K key)
        //{
        //    CacheRecord<V> retRecord;
        //    try
        //    {
        //            return cls[key].Get(); // Thread Safety - É??? Não me parece...
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        retRecord = new CacheRecord<V>();
        //        try
        //        {
        //            //Entre a Exception e a entrada aqui, abriu-se a janela!!! Alguém já pode ter inserido a mesma chave
        //            cls.Add(key, retRecord);
        //            retRecord.Set(sm(key));
        //        }
        //        catch (ArgumentException)
        //        {
        //            retRecord = cls[key];
        //        }

        //        return retRecord.Get();
        //    }
        //}

        //public V Get(K key)
        //{//Sugestão
        //    CacheRecord<V> retRecord = null;
        //    lock (monitor)
        //    {
        //        try
        //        {
        //            retRecord = cls[key];
        //        }
        //        catch (KeyNotFoundException)
        //        {
        //            cls[key] = retRecord; // atenção que cls[key] é sempre null. A referencia é copiada por valor. no caso 0 ( NULL ) 
        //        }
        //    }
        //    if (retRecord == null)
        //    {
        //        retRecord = new CacheRecord<V>();
        //        retRecord.Set(sm(key));
        //    }

        //    return retRecord.Get();
        //}

        public V Get(K key) // sugestão 001
        {
            CacheRecord<V> record = null;
            bool setValue         = false;

            lock (monitor)
            {
                try
                {
                    record = cls[key];
                }
                catch (KeyNotFoundException)
                {
                    record    = new CacheRecord<V>();
                    setValue  = true;
                    cls.Add(key, record );                    
                }
            }

            if (setValue)
            {
                record.Set(sm(key));
            }

            return record.Get();
        }

        public V GetII(K key) 
        {
            // sugestão 002 
            //              Com esta implementação tenta-se fugir ao estrangulamento na leitura da cls
            //              Utilização do lock para escrita.

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


        //Extension Methods - C#3.5
        //void PurgeCache(Object o)
        //{
        //    lock (monitor)
        //    {
        //        long lt = DateTimeHelper.CurrentTicks + rlt; //lt == Life Time
        //        cls.Where(p => p.Value.LastAccessTime < lt).Select(p => cls.Remove(p.Key));
        //    }            
        //}
        
        void PurgeCache(Object o)
        {
            lock (monitor)
            {
                long lt = DateTimeHelper.CurrentTicks - rlt; //lt == Life Time
                try
                {
                    foreach (KeyValuePair<K, CacheRecord<V>> keyPair in cls)
                    {
                        if (keyPair.Value != null && keyPair.Value.LastAccessTime < lt)
                            cls.Remove(keyPair.Key);
                    }
                }
                finally { }
            }
        }
    }
}

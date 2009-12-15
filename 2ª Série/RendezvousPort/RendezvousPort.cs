using System;
using System.Collections.Generic;
using System.Threading;

namespace RendezvousPort
{
    public class RendezvousPort<T> where T : class
    {
        private LinkedList<Task> waiting;
        private LinkedList<Task> queue;

        private class Task
        {
            public T obj = null;            // Object over which the server will operate
            public bool signaled = false;   // Flag that enables the server to proceed
            public Thread server = null;    // A reference to the servers thread
        }

        public RendezvousPort()
        {
            waiting = new LinkedList<Task>();   // List of server tasks waiting for requests
            queue = new LinkedList<Task>();     // List of client tasks waiting for servers
        }

        public T RequestService(T obj)
        {
            return RequestService(obj, -1);
        }

        /**
         * Handles a service request called by a Client to be preformed within the indicated timeout.
         * Thorws ThreadInterruptedException in case the thread was interrupted.
         * throws TimeoutException in case the request was handled outside the given period.
         */
        public T RequestService(T obj, long timeout)
        {
            //Creates a wrapper for the request's object
            Task _task;

            lock (queue)
            {
                //If there are no requests and there are servers available deliver directly to first server
                if (waiting.Count > 0 && queue.Count == 0)
                {
                    //Reference server task, assign obj, signal and pulse it
                    _task = waiting.First.Value;
                    waiting.RemoveFirst();
                    _task.signaled = true;
                    _task.obj = obj;
                    SyncUtils.Pulse(queue, _task);
                }
                //Otherwise enqueue client task in queue's last position
                else
                {
                    _task = new Task();
                    _task.obj = obj;
                    queue.AddLast(_task);
                }

                try
                {
                    //Wait until server pulses obj, indicating operation completed
                    SyncUtils.Wait(queue, obj, timeout);
                }
                catch (ThreadInterruptedException ie)
                {
                    //When interrupt is detected by SyncUtils.Wait terminate server thread
                    _task.server.Interrupt();
                    queue.Remove(_task);
                    throw ie;
                }
                catch (TimeoutException te)
                {
                    //when timeout is detected by SyncUtils.Wait terminate server thread
                    _task.server.Interrupt();
                    queue.Remove(_task);
                    throw te;
                }

                return obj;
            }
        }

        public T AcceptService()
        {
            return AcceptService(-1);
        }

        public T AcceptService(long timeout)
        {
            Task _task;

            lock (queue)
            {
                //If there are requests pending and no servers then grab first client task
                if (queue.Count > 0 && waiting.Count == 0)
                {
                    //Although return is imminent, this server thread should be assigned to the task
                    //so that the Client can interrupt it
                    _task = queue.First.Value;
                    _task.server = Thread.CurrentThread;
                    queue.RemoveFirst();
                    return (_task.obj);
                }

                //Otherwise create a task that waits for requests and add to waiting queue
                _task = new Task();
                _task.server = Thread.CurrentThread;
                waiting.AddLast(_task);

                while (true)
                {
                    try
                    {
                        SyncUtils.Wait(queue, _task, timeout);
                        //When awake verify if it was signaled
                        if (_task.signaled)
                        {
                            _task.signaled = false;
                            return _task.obj;
                        }
                    }
                    catch (ThreadInterruptedException ie)
                    {
                        //If interruption occurred verify if a request was delivered
                        if (_task.signaled)
                        {
                            //If so activate thread interrupt flag and returns the object
                            Thread.CurrentThread.Interrupt();
                            return _task.obj;
                        }
                        //Otherwise remove task from waiting and throws exception
                        waiting.Remove(_task);
                        throw ie;
                    }
                    catch (TimeoutException te)
                    {
                        //If timeout occurred verify if a request was delivered
                        if (_task.signaled)
                        {
                            //If so return the object
                            return _task.obj;
                        }
                        //Otherwise remove task from waiting and throw exception
                        waiting.Remove(_task);
                        throw te;
                    }
                }
            }
        }

        public void CompleteService(T obj)
        {
            lock (queue)
            {
                //Inform Client of request pulsing the object in which it's waiting on
                SyncUtils.Pulse(queue, obj);
            }
        }
    }
}

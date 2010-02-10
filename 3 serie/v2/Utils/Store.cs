/*
 * INSTITUTO SUPERIOR DE ENGENHARIA DE LISBOA
 * Licenciatura em Engenharia Informática e de Computadores
 *
 * Programação Concorrente - Inverno de 2009-2010
 * Paulo Pereira
 *
 * Código base para a 3ª Série de Exercícios.
 *
 */

using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Utils
{
    /// <summary>
    /// Singleton class that contains information regarding the tracked files, namely, the files' names
    /// and locations.
    /// 
    /// NOTE: This implementation is not thread-safe.
    /// </summary>
    public class Store
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static readonly Store _instance = null;


        static Store() //
        {
            _instance = new Store();  // 
        }


        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static Store Instance
        {
            get { return _instance; }
        }

        #region Instance members

        /// <summary>
        /// The dictionary instance that holds the tracked files information.
        /// </summary>
        private readonly Dictionary<string, HashSet<IPEndPoint>> _store;

        /// <summary>
        /// Empty array instance used to report the absence of tracking information for a particular file.
        /// </summary>
        private readonly IPEndPoint[] noLocations;

        /// <summary>
        /// Initiates the store instance.
        /// </summary>
        private Store()
        {
            _store = new Dictionary<string, HashSet<IPEndPoint>>();
            noLocations = new IPEndPoint[0];
        }

        /// <summary>
        /// Gives information about the existence of a file being hosted.
        /// </summary>
        /// <param name="key">Name of the file</param>
        /// <returns>A boolean value indicating if the file is registered.</returns>
        public bool ContainsKey(string key)
        {
            lock (_store)
            {
                return _store.ContainsKey(key);
            }
        }

        /// <summary>
        /// Registers the given file as being hosted at the given location. 
        /// </summary>
        /// <param name="fileName">The file's name.</param>
        /// <param name="client">The file's location.</param>
        public void Register(string fileName, IPEndPoint client)
        {
            // RN - Deixa de ser necessário
            //HashSet<IPEndPoint> fileHosts = null;

            lock (_store) // RN - Alterado para ganhar lock antes de read
            {
                if (!_store.ContainsKey(fileName))
                    ////_store[fileName] = (fileHosts = new HashSet<IPEndPoint>());
                    //_store[fileName] = (fileHosts = new HashSet<IPEndPoint>()); //
                    _store[fileName] = new HashSet<IPEndPoint> { client }; // RN
                else
                    //fileHosts = _store[fileName];
                    _store[fileName].Add(client); // RN
            }

            // RN - Para ter lock o mínimo de tempo e porque pode acontecer
            //      o mundo entre os blocos de lock é feita a afectação no if
            //lock (_store[fileName])
            //{
            //    fileHosts.Add(client);
            //}
        }

        /// <summary>
        /// Removes the given location for the given file (if both exist).
        /// </summary>
        /// <param name="fileName">The file's name.</param>
        /// <param name="client">The location of the hosting client.</param>
        /// <returns>A boolean value indicating if the file's location as been unregistered successfully.</returns>
        public bool Unregister(string fileName, IPEndPoint client)
        {
            bool result;
            // Is file being tracked?

            lock (_store) // RN - Adquire o lock antes de read
            {
                if (!_store.ContainsKey(fileName))
                    return false;

                // File locations are being tracked. Unregister client location.
                HashSet<IPEndPoint> locations = _store[fileName];
                result = locations.Remove(client); // RN - Manteve-se por estar com o lock
                //bool result = false; //

                // RN - Comentado porque na mudança de lock pode acontecer o mundo
                //lock (_store[fileName]) //
                //{
                //    result = locations.Remove(client); // 
                //}

                if (result && locations.Count == 0)
                    // Last client hosting the tracked file. Remove it from the store.
                    _store.Remove(fileName);

                // RN - Comentado porque na mudança de lock pode acontecer o mundo
                //lock (_store) //
                //{
                //    _store.Remove(fileName); // 
                //}
            }

            return result;
        }

        /// <summary>
        /// Gets the names of the files currently being tracked.
        /// </summary>
        /// <returns>An array with the tracked files' names.</returns>
        public string[] GetTrackedFiles()
        {
            ////return _store.Keys.ToArray();            

            string[] rObject = null; //
            lock (_store) //
            {
                rObject = _store.Keys.ToArray(); //
            }

            return rObject; //

        }

        /// <summary>
        /// Gets the locations of the given file.
        /// </summary>
        /// <param name="fileName">The file's name.</param>
        /// <returns>An array with the tracked files' locations.</returns>
        public IPEndPoint[] GetFileLocations(string fileName)
        {
            IPEndPoint[] locations = noLocations;
            lock (_store[fileName]) // RN - Adquire o lock antes do read
            {
                if (_store.ContainsKey(fileName))
                    ////locations = _store[fileName].ToArray();
                    locations = _store[fileName].ToArray(); // 
            }
            return locations;
        }

        #endregion
    }
}

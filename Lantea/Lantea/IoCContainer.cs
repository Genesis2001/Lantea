// -----------------------------------------------------------------------------
//  <copyright file="IoCContainer.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea
{
    using System;
    using System.Collections.Generic;
    using Common;

    public class IoCContainer : IIoCContainer
    {
        private readonly object mutex = new object();

        private readonly Dictionary<Type, object> instances = new Dictionary<Type, object>();
        private readonly Dictionary<Type, Type> map         = new Dictionary<Type, Type>();

        #region Implementation of IIoCContainer

        public void RegisterContract<T>()
        {
            lock (mutex)
            {
                if (!map.ContainsKey(typeof (T)))
                {
                    map.Add(typeof (T), typeof (T));
                }
            }
        }

        public void RegisterContract<T, TAs>() where TAs : T
        {
            lock (mutex)
            {
                if (!map.ContainsKey(typeof (T)))
                {
                    map.Add(typeof (T), typeof (TAs));
                }
            }
        }

        public void RegisterContract<T>(T contract, bool @override = false)
        {
            lock (mutex)
            {
                if (!map.ContainsKey(typeof (T)))
                {
                    map.Add(typeof (T), typeof (T));
                }

                if (!instances.ContainsKey(typeof (T)) || @override)
                {
                    instances[typeof (T)] = contract;
                }
            }
        }

        public void RegisterContract<T, TAs>(TAs contract, bool @override = false) where TAs : T
        {
            lock (mutex)
            {
                Type type;
                if (!map.TryGetValue(typeof (T), out type))
                {
                    map.Add(typeof (T), type = typeof (TAs));
                }

                if (!instances.ContainsKey(type) || @override)
                {
                    instances[type] = contract;
                }
            }
        }

        public T RetrieveContract<T>() where T : new()
        {
            lock (mutex)
            {
                Type type;
                if (!map.TryGetValue(typeof (T), out type))
                {
                    throw new KeyNotFoundException(
                        "Unable to retrieve a reference to the specified type. It doesn't exist in the internal map.");
                }

                object instance;
                if (!instances.TryGetValue(type, out instance))
                {
                    // type is registered, but no contract has been initialized.
                    instance = Activator.CreateInstance(typeof (T));
                }

                return (T)instance;
            }
        }

        public TAs RetrieveContract<T, TAs>() where TAs : T, new()
        {
            lock (mutex)
            {
                Type type;
                if (!map.TryGetValue(typeof (T), out type))
                {
                    throw new KeyNotFoundException(
                        "Unable to retrieve a reference to the specified type. It doesn't exist in the internal map.");
                }

                if (!typeof (T).IsAssignableFrom(typeof (TAs)))
                {
                    throw new ArgumentException(String.Format("The type '{0}' is not derived from '{1}'.",
                        typeof (TAs).FullName,
                        typeof (T).FullName));
                }

                object instance;
                if (!instances.TryGetValue(type, out instance))
                {
                    // type is registered, but no contract has been initialized.
                    instance = Activator.CreateInstance(typeof (TAs));
                }

                return (TAs)instance;
            }
        }

        #endregion
    }
}

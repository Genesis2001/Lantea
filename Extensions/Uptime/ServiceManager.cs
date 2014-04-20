// -----------------------------------------------------------------------------
//  <copyright file="ServiceManager.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Services;

	public class ServiceManager : IEnumerable<Service>
	{
		private static ServiceManager instance;

		/// <summary>
		/// Represents a singleton instance of the ServiceManager class.
		/// </summary>
		public static ServiceManager Instance
		{
			get { return instance ?? (instance = new ServiceManager()); }
		}

		private readonly Dictionary<String, String> map = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
		private readonly HashSet<Service> services = new HashSet<Service>();

		public Service CreateService(String key)
		{
			String fullname;
			if (!map.TryGetValue(key, out fullname)) return null;

			// not going to worry about exceptions here
			// because it's a private method and the obvious
			// ones aren't likely to occur since they're not
			// loaded by user-interaction.

			Type type = Type.GetType(fullname);
			return type != null ? (Service)Activator.CreateInstance(type) : null;
		}

		public void MapService<T>(String key) where T : Service
		{
			if (!map.ContainsKey(key))
			{
				map.Add(key, typeof (T).FullName);
			}
		}

		public void Register<T>(T service) where T : Service
		{
			if (!services.Any(x => ReferenceEquals(service, x)))
			{
				services.Add(service);
			}
		}

		#region Implementation of IEnumerable

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<Service> GetEnumerator()
		{
			return services.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)services).GetEnumerator();
		}

		#endregion
	}
}

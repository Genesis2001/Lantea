// -----------------------------------------------------------------------------
//  <copyright file="ServiceManager.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime
{
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using Services;

	public class ServiceManager
	{
		private readonly ConcurrentBag<IService> services = new ConcurrentBag<IService>(); 

		public IEnumerable<IService> Services
		{
			get { return services; }
		}

		public void Register<T>(T service) where T : IService
		{
			if (!services.Any(x => ReferenceEquals(x, service)))
			{
				services.Add(service);
			}
		}

		public void Unregister<T>(T service) where T : IService
		{
			if (services.Any(x => ReferenceEquals(x, service)))
			{
				
			}
		}
	}
}

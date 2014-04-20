// -----------------------------------------------------------------------------
//  <copyright file="Check.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime
{
	using System;
	using System.ComponentModel.Composition;
	using Lantea.Common.Extensibility;
	using Lantea.Common.IO;
	using Services;

	[Export(typeof (IModule))]
	[Module(Author = "Zack Loveless", Name = ModuleName, ModuleType = ModuleType.VENDOR, Version = ModuleVersion)]
	public class Uptime : ModuleCore
	{
		private const String ModuleName    = "Uptime";
		private const String ModuleDesc    = "Service Check Module";
		private const String ModuleVersion = "1.0";
		
		private Int32 defaultRetries;
		private Int32 defaultTimeout;

		[ImportingConstructor]
		public Uptime([Import] IBotCore bot, [Import] Configuration config) : base(bot, config)
		{
			defaultRetries = 0;
			defaultTimeout = 0;
		}

		#region Overrides of ModuleCore

		public override string Author
		{
			get { return "Zack Loveless"; }
		}

		public override string Description
		{
			get { return ModuleDesc; }
		}

		public override string Name
		{
			get { return ModuleName; }
		}

		public override string Version
		{
			get { return ModuleVersion; }
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.VENDOR; }
		}

		#endregion

		#region Overrides of ModuleCore

		public override void Load()
		{
			//Block uptime = Config.GetBlock("uptime");
			Block uptime = Config.GetBlock(this);

			if (uptime != null)
			{
				//ServiceManager.Instance.MapService<NullService>("none");
				ServiceManager.Instance.MapService<SshService>("ssh");
				ServiceManager.Instance.MapService<HttpService>("http");
				ServiceManager.Instance.MapService<MySqlService>("mysql");
				
				defaultRetries = uptime.Get("max_retries", 3);
				defaultTimeout = uptime.Get("timeout", 30);

				for (Int32 i = 0; i < uptime.CountBlock("service"); ++i)
				{
					Block b = uptime.GetBlock("service", i);

					// Get the type of this service.
					String type   = b.Get("type", "none");

					// Load some defaults first, since the initialize method won't actually have access to the parent block.
					Int32 retries = b.Get("max_retries", defaultRetries);
					Int32 timeout = b.Get("timeout", defaultTimeout);

					Service s = ServiceManager.Instance.CreateService(type);
					if (s != null)
					{
						s.RetryAttempts = retries;
						s.Timeout       = timeout;

						s.DisplayName = b.Get<String>("display");
						s.HostName    = b.Get<String>("hostname");
						s.Port        = b.Get<Int32>("port");

						s.Channels    = b.Get<String>("channels").Split(' ');

						s.Initialize(b.Data);

						ServiceManager.Instance.Register(s);
					}
				}
			}

			base.Load();
		}

		#endregion
	}
}

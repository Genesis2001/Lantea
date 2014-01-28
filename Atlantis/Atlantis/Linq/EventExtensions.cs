// -----------------------------------------------------------------------------
//  <copyright file="EventExtensions.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------
namespace Atlantis.Linq
{
	using System;

	public static partial class Extensions
	{
		public static void Raise(this EventHandler source, object sender, EventArgs args)
		{
			if (source != null) source(sender, args);
		}

		public static void Raise<TEventArgs>(this EventHandler<TEventArgs> source, object sender, TEventArgs args) where TEventArgs : EventArgs
		{
			if (source != null) source(sender, args);
		}
	}
}

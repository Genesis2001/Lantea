// -----------------------------------------------------------------------------
//  <copyright file="IIoCContainer.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common
{
	public interface IIoCContainer
	{
	    void RegisterContract<T>();

	    void RegisterContract<T, TAs>() where TAs : T;

        void RegisterContract<T>(T contract, bool @override = false);

	    void RegisterContract<T, TAs>(TAs contract, bool @override = false) where TAs : T;

	    T RetrieveContract<T>() where T : new();

	    TAs RetrieveContract<T, TAs>() where TAs : T, new();
	}
}

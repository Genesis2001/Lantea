// -----------------------------------------------------------------------------
//  <copyright file="SettingsFixtureHelper.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.UnitTests.Helpers
{
	using System.IO;
	using System.Text;

	public static class SettingsFixtureHelper
	{
		private const string XmlString = @"<Settings>
	<Core>
		<Connection Host=""irc.unifiedtech.org"" Port=""6697"" Nick=""Lantea"" Retry=""60"">
			<Options>
				<Secure CertificatePath=""~/.lantea/lantea.pem"" CertificateKeyPath=""~/.lantea/lantea.key"" />
			</Options>
		</Connection>
	</Core>
</Settings>";

		public static Stream GetMockFileStream()
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(XmlString));
		}
	}
}

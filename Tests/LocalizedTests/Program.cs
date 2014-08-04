// -----------------------------------------------------------------------------
//  <copyright file="Program.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace LocalizedTests
{
    using System;
    using Atlantis.Net.Query;

    public class Program
    {
        public static void Main(string[] args)
        {
            TS3Client client = null;
            try
            {
                client = new TS3Client
                                   {
                                       Host       = "ts.cncfps.com",
                                       QueryPort  = 26425,
                                       ServerPort = 9987,
                                       Login      = "Genesis",
                                       Password   = "Ij7IEvRN",
                                   };

                client.Connect();



                Console.Write("Press <ENTER> to continue...");
                Console.ReadLine();
            }
            finally
            {
                if (client != null)
                {
                    client.Disconnect();
                }
            }
        }
    }
}

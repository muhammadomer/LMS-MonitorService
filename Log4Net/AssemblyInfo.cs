#region Apache License
//
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System.Reflection;
using System.Runtime.CompilerServices;

#if (!SSCLI)
//
// log4net makes use of static methods which cannot be made com visible
//
[assembly: System.Runtime.InteropServices.ComVisible(false)]
#endif

//
// log4net is CLS compliant
//
[assembly: System.CLSCompliant(true)]

#if (!NETCF)
//
// If log4net is strongly named it still allows partially trusted callers
//
[assembly: System.Security.AllowPartiallyTrustedCallers]
#endif

#if (NET_4_0)
//
// Allows partial trust applications (e.g. ASP.NET shared hosting) on .NET 4.0 to work
// given our implementation of ISerializable.
//
[assembly: System.Security.SecurityRules(System.Security.SecurityRuleSet.Level1)]
#endif

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//

#if (CLI_1_0)
[assembly: AssemblyTitle("Apache log4net for CLI 1.0 Compatible Frameworks")]
#elif (NET_1_0)
[assembly: AssemblyTitle("Apache log4net for .NET Framework 1.0")]
#elif (NET_1_1)
[assembly: AssemblyTitle("Apache log4net for .NET Framework 1.1")]
#elif (NET_4_0)
#if CLIENT_PROFILE
[assembly: AssemblyTitle("Apache log4net for .NET Framework 4.0 Client Profile")]
#else
[assembly: AssemblyTitle("log4net")]
#endif // Client Profile
#elif (NET_2_0)
#if CLIENT_PROFILE
[assembly: AssemblyTitle("Apache log4net for .NET Framework 3.5 Client Profile")]
#else
[assembly: AssemblyTitle("Apache log4net for .NET Framework 2.0")]
#endif // Client Profile
#elif (NETCF_1_0)
[assembly: AssemblyTitle("Apache log4net for .NET Compact Framework 1.0")]
#elif (NETCF_2_0)
[assembly: AssemblyTitle("Apache log4net for .NET Compact Framework 2.0")]
#elif (MONO_1_0)
[assembly: AssemblyTitle("Apache log4net for Mono 1.0")]
#elif (MONO_2_0)
[assembly: AssemblyTitle("Apache log4net for Mono 2.0")]
#elif (SSCLI_1_0)
[assembly: AssemblyTitle("Apache log4net for Shared Source CLI 1.0")]
#elif (CLI_1_0)
[assembly: AssemblyTitle("Apache log4net for CLI Compatible Frameworks")]
#elif (NET)
[assembly: AssemblyTitle("Apache log4net for .NET Framework")]
#elif (NETCF)
[assembly: AssemblyTitle("Apache log4net for .NET Compact Framework")]
#elif (MONO)
[assembly: AssemblyTitle("Apache log4net for Mono")]
#elif (SSCLI)
[assembly: AssemblyTitle("Apache log4net for Shared Source CLI")]
#else
[assembly: AssemblyTitle("Apache log4net")]
#endif

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Retail")]
#endif

[assembly: AssemblyProduct("vRECORD")]
[assembly: AssemblyDefaultAlias("log4net")]
[assembly: AssemblyCulture("")]		
		
//
// In order to sign your assembly you must specify a key to use. Refer to the 
// Microsoft .NET Framework documentation for more information on assembly signing.
//
// Use the attributes below to control which key is used for signing. 
//
// Notes: 
//   (*) If no key is specified, the assembly is not signed.
//   (*) KeyName refers to a key that has been installed in the Crypto Service
//       Provider (CSP) on your machine. KeyFile refers to a file which contains
//       a key.
//   (*) If the KeyFile and the KeyName values are both specified, the 
//       following processing occurs:
//       (1) If the KeyName can be found in the CSP, that key is used.
//       (2) If the KeyName does not exist and the KeyFile does exist, the key 
//           in the KeyFile is installed into the CSP and used.
//   (*) In order to create a KeyFile, you can use the sn.exe (Strong Name) utility.
//       When specifying the KeyFile, the location of the KeyFile should be
//       relative to the project output directory which is
//       %Project Directory%\obj\<configuration>. For example, if your KeyFile is
//       located in the project directory, you would specify the AssemblyKeyFile 
//       attribute as [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//
#if STRONG && (CLI_1_0 || NET_1_0 || NET_1_1 || NETCF_1_0 || SSCLI)
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile(@"..\..\..\log4net.snk")]
#endif
// We do not use a CSP key for strong naming
// [assembly: AssemblyKeyName("")]

[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyCompanyAttribute("Vurella")]
[assembly: AssemblyCopyrightAttribute("Copyright � Vurella 2016")]

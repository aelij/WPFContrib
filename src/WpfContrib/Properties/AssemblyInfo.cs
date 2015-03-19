using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Markup;

[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: AllowPartiallyTrustedCallers]
[assembly: AssemblyTitle("WPF Contrib")]
[assembly: AssemblyDescription("WPF Contrib")]
[assembly: AssemblyCompany("Eli Arbel")]
[assembly: AssemblyProduct("WPF Contrib")]
[assembly: AssemblyCopyright("Copyright 2015")]
[assembly: AssemblyVersion("2.2.0.0")]
[assembly: AssemblyFileVersion("2.2.0.0")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, // theme-specific
    ResourceDictionaryLocation.SourceAssembly // generic
    )]
[assembly: XmlnsPrefix("http://schemas.codeplex.com/wpfcontrib/xaml/presentation", "av")]
[assembly: XmlnsDefinition("http://schemas.codeplex.com/wpfcontrib/xaml/presentation", "Avalon.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.codeplex.com/wpfcontrib/xaml/presentation", "Avalon.Windows.Converters")]
[assembly: XmlnsDefinition("http://schemas.codeplex.com/wpfcontrib/xaml/presentation", "Avalon.Windows.Media.Animation")]
[assembly: XmlnsDefinition("http://schemas.codeplex.com/wpfcontrib/xaml/presentation", "Avalon.Windows.Media.Effects")]
[assembly: XmlnsDefinition("http://schemas.codeplex.com/wpfcontrib/xaml/presentation", "Avalon.Windows.Utility")]

using System;
using System.Windows;

namespace AirportApp;

internal static class Program {
    [STAThread]
    private static void Main() => new Application().Run(new ManagementWindow());
}

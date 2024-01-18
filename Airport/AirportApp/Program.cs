using System;
using System.Windows;

namespace WpfExp;

internal static class Program {
    [STAThread] private static void Main() => new Application().Run(new Window());
}

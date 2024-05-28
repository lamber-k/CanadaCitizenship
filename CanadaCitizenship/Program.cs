using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace CanadaCitizenship
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args is [string locale] && TryGetCultureInfo(locale, out CultureInfo? target))
            {
                CultureInfo.CurrentCulture = target;
                CultureInfo.CurrentUICulture = target;
            }
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Main());
        }

        private static bool TryGetCultureInfo(string locale, [NotNullWhen(true)]out CultureInfo? target)
        {
            try
            {
                target = CultureInfo.GetCultureInfo(locale);
                return true;
            }
            catch (CultureNotFoundException)
            {
                target = null;
                return false;
            }
        }
    }
}
namespace GUI
{
	using System;
	using System.Threading;
	using System.Windows.Forms;

	public class GuiExceptionHandler
	{
		/// <summary>
		/// Shows message box instead of unhandled exceptions.
		/// </summary>
		public static void SetupCatching()
		{
			if (!AppDomain.CurrentDomain.FriendlyName.EndsWith("vshost.exe"))
			{
				// Add the event handler for handling UI thread exceptions to the event.
				Application.ThreadException += UIThreadException;

				// Set the unhandled exception mode to force all Windows Forms errors
				// to go through our handler.
				Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

				// Add the event handler for handling non-UI thread exceptions to the event. 
				AppDomain.CurrentDomain.UnhandledException += UnhandledException;
			}
		}

		private static void UIThreadException(object sender, ThreadExceptionEventArgs e)
		{
			MessageBox.Show(e.Exception.Message);
		}

		private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show("Fatal error");
		}
	}
}
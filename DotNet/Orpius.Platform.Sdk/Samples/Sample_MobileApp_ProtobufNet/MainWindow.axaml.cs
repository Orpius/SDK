using Avalonia.Controls;
using Avalonia.Input;

namespace SampleProtobufNet
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			DataContext = new MainWindowViewModel();
		}

		MainWindowViewModel? ViewModel => (MainWindowViewModel?)DataContext;

		void PromptBox_OnKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (e.KeyModifiers == KeyModifiers.Shift)
				{
					e.Handled = false;

					/* New line. */
					//base.OnKeyDown(new KeyEventArgs { Key = e.Key, KeyDeviceType = e.KeyDeviceType, KeySymbol = e.KeySymbol, PhysicalKey = e.PhysicalKey });
					return;
				}
				else if (e.KeyModifiers == KeyModifiers.None)
				{
					e.Handled = true;

					ViewModel?.SendMessageCommand.Execute(null);
					return;
				}
			}

			base.OnKeyDown(e);
		}
	}
}
using System;
using System.Globalization;

using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;

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

		void PromptBox_OnEnterPressed(object? sender, PromptEnterPressedEventArgs e)
		{
			e.Handled = true;
			ViewModel?.SendMessageCommand.Execute(null);
		}
	}

	public class BoolToSuccessConverter : IValueConverter
	{
		public string TrueText  { get; set; } = ((char)0x2714) + " Success";
		public string FalseText { get; set; } = ((char)0x2717) + " Failed";
		public string NullText  { get; set; } = "...";

		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			switch (value as bool?)
			{
				case true:  return TrueText;
				case false: return FalseText;
				default:    return NullText;
			}
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}

	public class PromptBox : TextBox
	{
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (e.KeyModifiers == KeyModifiers.Shift)
				{
					e.Handled = true;

					/* New line. */
					base.OnKeyDown(new KeyEventArgs { Key = e.Key, KeyDeviceType = e.KeyDeviceType, KeySymbol = e.KeySymbol, PhysicalKey = e.PhysicalKey });
					return;
				}

				if (e.KeyModifiers == KeyModifiers.None)
				{
					e.Handled = true;

					EnterPressed?.Invoke(this, new PromptEnterPressedEventArgs());
					return;
				}
			}

			base.OnKeyDown(e);
		}

		public event EventHandler<PromptEnterPressedEventArgs>? EnterPressed;
	}

	public class PromptEnterPressedEventArgs : RoutedEventArgs
	{
	}
}
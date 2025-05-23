using Avalonia.Controls;

namespace SampleProtobufNet
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			DataContext = new MainWindowViewModel();
		}
	}
}
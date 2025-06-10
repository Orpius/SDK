using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Avalonia.Threading;

namespace SampleProtobufNet
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChangedEventHandler? temp = PropertyChanged;

			if (temp != null)
			{
				Dispatcher.UIThread.InvokeIfRequired(
					() => temp.Invoke(this, new PropertyChangedEventArgs(propertyName)));
			}
		}

		protected bool Set<TField>(
			ref TField field, TField newValue,
			[CallerMemberName] string propertyName = "")
		{
			if (!EqualityComparer<TField>.Default.Equals(field, newValue))
			{
				field = newValue;
				OnPropertyChanged(propertyName);
				return true;
			}

			return false;
		}
	}

	static class DispatcherExtensions
	{
		internal static void InvokeIfRequired(this Dispatcher dispatcher, Action action)
		{
			if (dispatcher.CheckAccess())
			{
				action();
			}
			else
			{
				dispatcher.Invoke(action);
			}
		}
	}
}
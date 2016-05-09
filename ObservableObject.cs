using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SimpleMvvm.Annotations;

namespace SimpleMvvm
{
	public class ObservableObject
		: INotifyPropertyChanged
	{
		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region SetValue

		[NotifyPropertyChangedInvocator]
		protected bool SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			return SetValue(ref field, value, (Action<T, T>)null, propertyName);
		}

		[NotifyPropertyChangedInvocator]
		protected bool SetValue<T>(ref T field, T value, Action successHandler, [CallerMemberName] string propertyName = null)
		{
			var handler = successHandler == null
				? (Action<T, T>)null
				: (oldValue, newValue) => successHandler();

			return SetValue(ref field, value, handler, propertyName);
		}

		[NotifyPropertyChangedInvocator]
		protected bool SetValue<T>(ref T field, T value, Action<T> successHandler, [CallerMemberName] string propertyName = null)
		{
			var handler = successHandler == null
				? (Action<T, T>)null
				: (oldValue, newValue) => successHandler(newValue);

			return SetValue(ref field, value, handler, propertyName);
		}

		[NotifyPropertyChangedInvocator]
		protected bool SetValue<T>(ref T field, T value, Action<T, T> successHandler, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
			{
				return false;
			}

			var currentValue = field;

			field = value;
			OnPropertyChanged(propertyName);

			successHandler?.Invoke(currentValue, value);

			return true;
		}

		#endregion
	}
}
using System;
using System.ComponentModel;

namespace SimpleMvvm.Common
{
	internal class WeakPropertyChangedListener
	{
		private WeakReference _weakEventListener;
		private INotifyPropertyChanged _notifyCollectionChanged;

		public WeakPropertyChangedListener(INotifyPropertyChanged notify, IWeakPropertyChangedListener eventListener)
		{
			if (notify == null
			    || eventListener == null)
			{
				return;
			}

			_notifyCollectionChanged = notify;
			_notifyCollectionChanged.PropertyChanged += PropertyChanged;

			_weakEventListener = new WeakReference(eventListener);
		}

		private void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var listener = _weakEventListener;
			if (listener == null)
			{
				return;
			}

			var eventListener = listener.Target as IWeakPropertyChangedListener;
			if (eventListener != null)
			{
				eventListener.EventHandler(sender, e);
				return;
			}

			Disconnect();
		}

		public void Disconnect()
		{
			var source = _notifyCollectionChanged;

			_notifyCollectionChanged = null;
			_weakEventListener = null;

			if (source == null)
			{
				return;
			}

			source.PropertyChanged -= PropertyChanged;
		}
	}
}
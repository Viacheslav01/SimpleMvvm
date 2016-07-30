using System;
using System.ComponentModel;
using System.Threading;

namespace SimpleMvvm.Common
{
	internal class PropertyChangedHandler
		: IPropertyChangedHandler, IDisposable, IWeakPropertyChangedListener
	{
		private INotifyPropertyChanged _source;
		private PropertyChangedEventHandler _handler;

		private WeakPropertyChangedListener _weakPropertyChangedListener;

		public PropertyChangedHandler(INotifyPropertyChanged source, Action<string> handler)
		{
			Guard.NotNull(source);
			Guard.NotNull(handler);

			_source = source;
			_handler = (o, e) => handler(e.PropertyName);

			Subscribe();
		}

		public PropertyChangedHandler(INotifyPropertyChanged source, string propertyName, Action handler)
		{
			Guard.NotNull(source);
			Guard.NotNullAndEmpty(propertyName);
			Guard.NotNull(handler);

			_source = source;
			_handler = (o, e) =>
			{
				if (string.Equals(propertyName, e.PropertyName))
				{
					handler();
				}
			};

			Subscribe();
		}

		void IWeakEventListener<PropertyChangedEventArgs>.EventHandler(object source, PropertyChangedEventArgs args)
		{
			_handler?.Invoke(source, args);
		}

		public void Subscribe()
		{
			var listener = Interlocked.Exchange(ref _weakPropertyChangedListener, new WeakPropertyChangedListener(_source, this));
			listener?.Disconnect();
		}

		public void Release()
		{
			var listener = Interlocked.Exchange(ref _weakPropertyChangedListener, null);
			listener?.Disconnect();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// ReSharper disable once UnusedParameter.Local
		private void Dispose(bool disposing)
		{
			Release();

			_source = null;
			_handler = null;
		}

		~PropertyChangedHandler()
		{
			Dispose(false);
		}
	}
}
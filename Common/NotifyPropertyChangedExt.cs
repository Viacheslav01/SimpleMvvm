using System;
using System.ComponentModel;
using SimpleMvvm.Annotations;

namespace SimpleMvvm.Common
{
	public static class NotifyPropertyChangedExt
	{
		public static IPropertyChangedHandler GetPropertyChangedHandler(this INotifyPropertyChanged source, Action<string> handler)
		{
			Guard.NotNull(handler);

			if (source == null)
			{
				return new NullPropertyChangedHandler();
			}

			return new PropertyChangedHandler(source, handler);
		}

		public static IPropertyChangedHandler GetPropertyChangedHandler<T>(this INotifyPropertyChanged source, [LocalizationRequired(false)] string propertyName, Action handler)
		{
			Guard.NotNullAndEmpty(propertyName);
			Guard.NotNull(handler);

			if (source == null)
			{
				return new NullPropertyChangedHandler();
			}

			return new PropertyChangedHandler(source, propertyName, handler);
		}
	}
}

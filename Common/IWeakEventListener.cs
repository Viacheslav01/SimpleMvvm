using System;

namespace SimpleMvvm.Common
{
	internal interface IWeakEventListener<T>
		where T : EventArgs
	{
		void EventHandler(object source, T args);
	}
}
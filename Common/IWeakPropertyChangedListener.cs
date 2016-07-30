using System.ComponentModel;

namespace SimpleMvvm.Common
{
	internal interface IWeakPropertyChangedListener
		: IWeakEventListener<PropertyChangedEventArgs>
	{ }
}
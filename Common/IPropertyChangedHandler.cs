namespace SimpleMvvm.Common
{
	public interface IPropertyChangedHandler
	{
		void Subscribe();
		void Release();
	}
}
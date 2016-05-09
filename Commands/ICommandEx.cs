using System.Windows.Input;

namespace SimpleMvvm.Commands
{
	public interface ICommandEx
		: ICommand
	{
		void RiseCanExecuteChanged();
	}
}

using System;

namespace SimpleMvvm.Commands
{
	public class DelegateCommand
		: Command
	{
		private readonly Action<object> _execute;
		private readonly Func<object, bool> _canExecute;

		public DelegateCommand(Action execute, Func<bool> canExecute = null)
		{
			Guard.NotNull(execute, nameof(execute));

			_execute = o => execute();
			if (canExecute != null)
			{
				_canExecute = o => canExecute();
			}
		}

		public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
		{
			Guard.NotNull(execute, nameof(execute));

			_execute = execute;
			_canExecute = canExecute;
		}

		public override bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute(parameter);
		}

		public override void Execute(object parameter)
		{
			if (!CanExecute(parameter))
			{
				return;
			}

			_execute(parameter);
		}

		protected override bool IsCanExecuteSupported => _canExecute != null;
	}
}
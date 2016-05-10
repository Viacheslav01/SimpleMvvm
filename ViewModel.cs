using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SimpleMvvm.Commands;

namespace SimpleMvvm
{
	public class ViewModel
		: ObservableObject
	{
		#region Properties

		#region IsBusy

		private bool _isBusy;

		public bool IsBusy
		{
			get { return _isBusy; }
			protected set { SetValue(ref _isBusy, value, UpdateCommandsState); }
		}

		#endregion

		#endregion

		#region Commands management

		private readonly ReaderWriterLockSlim _busyRelatedCommandsLock = new ReaderWriterLockSlim();
		private readonly List<WeakReference<ICommandEx>> _busyRelatedCommands = new List<WeakReference<ICommandEx>>();

		private void UpdateCommandsState()
		{
			using (_busyRelatedCommandsLock.UpgradeableReadLock())
			{
				for (var index = _busyRelatedCommands.Count - 1; index >= 0; index--)
				{
					ICommandEx command;
					if (!_busyRelatedCommands[index].TryGetTarget(out command))
					{
						using (_busyRelatedCommandsLock.WriteLock())
						{
							_busyRelatedCommands.RemoveAt(index);
						}

						return;
					}

					command.RiseCanExecuteChanged();
				}
			}
		}

		protected DelegateCommand GetCommand(ref DelegateCommand command, Action execute, Func<bool> canExecute = null, bool busyRelated = true)
		{
			if (command != null)
			{
				return command;
			}

			var fullCanExecute = canExecute;

			if (busyRelated)
			{
				fullCanExecute = () => !IsBusy && (canExecute?.Invoke() ?? true);
			}

			command = new DelegateCommand(execute, fullCanExecute);

			if (busyRelated)
			{
				using (_busyRelatedCommandsLock.WriteLock())
				{
					_busyRelatedCommands.Add(new WeakReference<ICommandEx>(command));
				}
			}

			return command;
		}

		protected DelegateCommand GetCommand(ref DelegateCommand command, Action<object> execute, Func<object, bool> canExecute = null, bool busyRelated = true)
		{
			if (command != null)
			{
				return command;
			}

			var fullCanExecute = canExecute;

			if (busyRelated)
			{
				fullCanExecute = o => !IsBusy && (canExecute?.Invoke(o) ?? true);
			}

			command = new DelegateCommand(execute, fullCanExecute);

			if (busyRelated)
			{
				using (_busyRelatedCommandsLock.WriteLock())
				{
					_busyRelatedCommands.Add(new WeakReference<ICommandEx>(command));
				}
			}

			return command;
		}

		protected AsyncCommand GetCommand(ref AsyncCommand command, Func<Task> execute, Func<bool> canExecute = null, bool busyRelated = true)
		{
			if (command != null)
			{
				return command;
			}

			var fullCanExecute = canExecute;

			if (busyRelated)
			{
				fullCanExecute = () => !IsBusy && canExecute.With(h => h(), true);
			}

			command = new AsyncCommand(execute, fullCanExecute);

			if (busyRelated)
			{
				using (_busyRelatedCommandsLock.WriteLock())
				{
					_busyRelatedCommands.Add(new WeakReference<ICommandEx>(command));
				}
			}

			return command;
		}

		protected AsyncCommand GetCommand(ref AsyncCommand command, Func<object, Task> execute, Func<object, bool> canExecute = null, bool busyRelated = true)
		{
			if (command != null)
			{
				return command;
			}

			var fullCanExecute = canExecute;

			if (busyRelated)
			{
				fullCanExecute = o => !IsBusy && canExecute.With(h => h(o), true);
			}

			command = new AsyncCommand(execute, fullCanExecute);

			if (busyRelated)
			{
				using (_busyRelatedCommandsLock.WriteLock())
				{
					_busyRelatedCommands.Add(new WeakReference<ICommandEx>(command));
				}
			}

			return command;
		}

		#endregion
	}
}

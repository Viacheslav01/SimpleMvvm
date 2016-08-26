using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SimpleMvvm.Common;

namespace SimpleMvvm
{
	public sealed class MessageHub
		: IMessageHub
	{
		private readonly ReaderWriterLockSlim _lockObject = new ReaderWriterLockSlim();
		private readonly Dictionary<Type, IHandlersManager> _managers = new Dictionary<Type, IHandlersManager>();

		public IDisposable Subscribe<TMessage>(Action<TMessage> handler)
		{
			Guard.NotNull(handler);

			IHandlersManager manager;

			using (_lockObject.UpgradeableReadLock())
			{
				if (!_managers.TryGetValue(typeof(TMessage), out manager))
				{
					manager = new HandlerManager<TMessage>();
					using (_lockObject.WriteLock())
					{
						_managers.Add(typeof(TMessage), manager);
					}
				}
			}

			return manager.Subscribe(handler);
		}

		public void Send<TMessage>(TMessage message)
		{
			IHandlersManager manager;

			using (_lockObject.ReadLock())
			{
				_managers.TryGetValue(typeof(TMessage), out manager);
			}

			manager?.Send(message);
		}

		public Task SendAsync<TMessage>(TMessage message)
		{
			return Task.Run(() => Send(message));
		}

		private interface IHandlersManager
		{
			IDisposable Subscribe(object handler);
			void Send(object message);
			void Clean();
		}

		private sealed class HandlerManager<TMessage>
			: IHandlersManager
		{
			private readonly ReaderWriterLockSlim _lockObject = new ReaderWriterLockSlim();
			private readonly HashSet<WeakReference<Action<TMessage>>> _handlers = new HashSet<WeakReference<Action<TMessage>>>();

			IDisposable IHandlersManager.Subscribe(object handler)
			{
				return Subscribe((Action<TMessage>)handler);
			}

			void IHandlersManager.Send(object message)
			{
				Send((TMessage)message);
			}

			void IHandlersManager.Clean()
			{
				Clean();
			}

			private IDisposable Subscribe(Action<TMessage> handler)
			{
				Guard.NotNull(handler);

				using (_lockObject.WriteLock())
				{
					var weakRefference = new WeakReference<Action<TMessage>>(handler);
					_handlers.Add(weakRefference);

					return new UnsubscribeHandler(handler, () => Unsubscribe(weakRefference));
				}
			}

			private void Unsubscribe(WeakReference<Action<TMessage>> weakRefference)
			{
				if (weakRefference == null)
				{
					return;
				}

				using (_lockObject.WriteLock())
				{
					_handlers.Remove(weakRefference);
				}
			}

			private void Send(TMessage message)
			{
				if (_handlers.Count == 0)
				{
					return;
				}

				var cleanRequired = false;
				List<Action<TMessage>> handlers;

				using (_lockObject.ReadLock())
				{
					if (_handlers.Count == 0)
					{
						return;
					}

					handlers = new List<Action<TMessage>>(_handlers.Count);

					foreach (var weakRefference in _handlers.ToArray())
					{
						Action<TMessage> handler;

						if (weakRefference.TryGetTarget(out handler))
						{
							handlers.Add(handler);
						}
						else
						{
							cleanRequired = true;
						}
					}
				}

				foreach (var handler in handlers)
				{
					try
					{
						handler(message);
					}
					catch (Exception ex)
					{
						Debug.WriteLine("Unable to send message: {0}", ex);
					}
				}

				if (cleanRequired)
				{
					Clean();
				}
			}

			private void Clean()
			{
				if (_handlers.Count == 0)
				{
					return;
				}

				using (_lockObject.UpgradeableReadLock())
				{
					if (_handlers.Count == 0)
					{
						return;
					}

					IDisposable writerLock = null;

					try
					{
						foreach (var weakRefference in _handlers.ToArray())
						{
							Action<TMessage> handler;

							if (!weakRefference.TryGetTarget(out handler))
							{
								_lockObject.WriteLockIfRequired(ref writerLock);
								_handlers.Remove(weakRefference);
							}
						}
					}
					finally
					{
						writerLock?.Dispose();
					}
				}
			}

			#region Nested types

			private class UnsubscribeHandler
				: IDisposable
			{
				private Action _unsubscribeAction;
				// Ссылка на обработчик необходимо, что бы предоствратить уничтожение делегата сборщиком мусора
				private Action<TMessage> _handler;

				public UnsubscribeHandler(Action<TMessage> handler, Action unsubscribeAction)
				{
					_handler = handler;
					_unsubscribeAction = unsubscribeAction;
				}

				public void Dispose()
				{
					_unsubscribeAction?.Invoke();
					
					_handler = null;
					_unsubscribeAction = null;
				}
			}

			#endregion
		}
	}
}

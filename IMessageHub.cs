using System;
using System.Threading.Tasks;

namespace SimpleMvvm
{
	public interface IMessageHub
	{
		IDisposable Subscribe<TMessage>(Action<TMessage> handler);
		void Send<TMessage>(TMessage message);
		Task SendAsync<TMessage>(TMessage message);
	}
}
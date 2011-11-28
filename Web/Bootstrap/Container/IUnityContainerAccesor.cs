
using Autofac;
namespace Web.Bootstrap.Container
{
	public interface IContainerAccessor
	{
		IContainer Container { get; }
	}
}
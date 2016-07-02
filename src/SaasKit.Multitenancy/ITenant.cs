namespace SaasKit.Multitenancy
{
	public interface ITenant<out TTenant>
	{
		TTenant Value { get; }
	}
}
namespace SaasKit.Multitenancy.Internal
{
	internal class Tenant<TTenant> : ITenant<TTenant>
	{
		public Tenant(TTenant value)
		{
			Value = value;
		}

		public TTenant Value { get; }
	}
}
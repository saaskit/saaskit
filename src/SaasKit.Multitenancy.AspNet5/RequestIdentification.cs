namespace SaasKit.Multitenancy.AspNet5
{
	public static class RequestIdentification
	{
		public static RequestIdentificationStrategy FromHostname()
		{
			return context => context.Request.Host.Value;
		}
	}
}


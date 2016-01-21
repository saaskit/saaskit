namespace SaasKit.Multitenancy.Samples.Mvc.AspNet5.StructureMap
{
	public interface IMessageService
	{
		string GetMessage();
	}

	public class MessageService : IMessageService
	{
		public string GetMessage()
		{
			return "Default Message from MessageService";
		}
	}

	public class OtherMessageService : IMessageService
	{
		public string GetMessage()
		{
			return "A completely different message from OtherMessageService";
		}
	}
}

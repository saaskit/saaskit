namespace AspNetMvcAuthSample
{
    public class AppTenant
    {
        public string Name { get; set; }
        public string[] Hostnames { get; set; }

        public string Id
        {
            get
            {
                return Name.Replace(" ", "").ToLower();
            }
        }
    }
}

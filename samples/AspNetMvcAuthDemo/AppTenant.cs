using System;

namespace AspNetMvcAuthSample
{
    public class AppTenant : IEquatable<AppTenant>
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

        public bool Equals(AppTenant other)
        {
            if (other == null)
            {
                return false;
            }

            return other.Name.Equals(Name);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AppTenant);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public string GoogleClientId { get; set; }
        public string GoogleClientSecret { get; set; }
    }
}

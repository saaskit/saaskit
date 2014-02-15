using System;

namespace SaasKit
{
    public class InstanceLifetimeOptions
    {
        public bool UseSlidingExpiration { get; set; }
        public TimeSpan Lifetime { get; set; }

        public static InstanceLifetimeOptions Default
        {
            get
            {
                return new InstanceLifetimeOptions
                {
                    Lifetime = TimeSpan.FromHours(6),
                    UseSlidingExpiration = true
                };
            }
        }
    }
}

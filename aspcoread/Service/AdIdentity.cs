using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace aspcoread.Service
{
    public class AdIdentity : IIdentity
    {
        public AdIdentity(string name)
        {
            this.AuthenticationType = "ad";
            IsAuthenticated = true;
            Name = name;
        }

        public string AuthenticationType { get; }
        public bool IsAuthenticated { get; }
        public string Name { get; }
    }
}

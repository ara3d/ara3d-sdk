using System;

namespace Ara3D.Utils
{
    public class UserData
    {
        public string UserName { get; set; } = Environment.UserName;
        public string UserDomainName { get; set; } = Environment.UserDomainName;
        public static UserData Default => new UserData();
    }
}   
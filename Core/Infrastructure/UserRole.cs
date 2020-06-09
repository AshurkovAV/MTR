using System;
using System.Linq;

namespace Core.Infrastructure
{
    [Flags]
    public enum UserRole : int
    {
        Admin = 1,
        InterTerritorial = 2,
        Insurance = 4,
        Economist = 8,
        User = 64,
    }

    public static class AccessLevel
    {
        public const int User = (int)(UserRole.User | UserRole.Admin | UserRole.InterTerritorial | UserRole.Economist);
        public const int Admin = (int)(UserRole.Admin);
        public const int InterTerritorial = (int)(UserRole.InterTerritorial | UserRole.Admin);
        public const int Insurance = (int)(UserRole.Insurance | UserRole.Admin);
        public const int Economist = (int)(UserRole.Economist | UserRole.Admin);

        public static bool CheckAccess(UserRole role, int access)
        {
            return ((int)role & access) != 0;
        }

        public static bool CheckAccess(UserRole role, params int[] access)
        {
            return access.Any(p => ((int)role & p) != 0);
        }
    }
}

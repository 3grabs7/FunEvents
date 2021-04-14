using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FunEvents
{
    public static class RoleHirarchy
    {
        public static IList<string> Roles { get; set; } = new List<string>()
        {
            "Jack", "Admin", "Organization", "Manager", "Assistant", "Attendee"
        };

        public static IList<Dictionary<string, List<int>>> RolesInheritance { get; set; } = new List<Dictionary<string, List<int>>>()
        {
            new Dictionary<string, List<int>>()
            {
                ["DunderRollen"] = {0, 6, 7}
            },
            new Dictionary<string, List<int>>()
            {
                ["AndraDunderRollen"] = {4, 5, 7}
            }
        };

        public static string GetHierarchy(string role)
        {
            int hirarchyPosition = Roles.IndexOf(role);
            string rolesCollection = "";
            for (int i = hirarchyPosition; i < Roles.Count; i++)
            {
                rolesCollection += i == Roles.Count - 1 ? Roles[i] : $"{Roles[i]}, ";
            }
            return rolesCollection;
        }

        public static string GetInheritence(string inherit)
        {
            List<int> inheritSpecificRoles = new List<int>();

            foreach (var item in RolesInheritance)
            {
                item.TryGetValue(inherit, out inheritSpecificRoles);
            }

            string rolesCollection = "";
            for (int i = 0; i < Roles.Count; i++)
            {
                if (inheritSpecificRoles.Contains(i))
                {
                    rolesCollection += i == Roles.Count - 1 ? Roles[i] : $"{Roles[i]}, ";
                }
            }
            return rolesCollection;
        }

    }
}

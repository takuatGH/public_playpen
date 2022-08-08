namespace PaxQ1.Models
{
    public class PaxRoleManager
    {
        private static Dictionary<int, string> Roles = new Dictionary<int, string>()
        {
            {1, "Security"},
            {2, "Admin"},
            {3, "Owner"}
        };

        public static ICollection<string> GetRoles(string role)
        {
            var accessLevel = Roles.SingleOrDefault(x => x.Value == role).Key;
            var authorizedRoles = new List<string>();
            for(int i = accessLevel; i > 0;  i--)
            {
                authorizedRoles.Add(Roles[i]);
            }
            return authorizedRoles;
        }
    }
}
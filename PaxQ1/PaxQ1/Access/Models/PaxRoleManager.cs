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
            var roleLevel = Roles.SingleOrDefault(x => x.Value == role).Key;
            var testList = new List<string>();
            for(int i = roleLevel; i > 0;  i--)
            {
                testList.Add(Roles[i]);
            }
            return testList;
        }
    }
}
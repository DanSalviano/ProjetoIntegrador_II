namespace PizzaDelivery.Extensions
{
    public static class StringExtensions
    {
        public static (string FirstName, string LastName) TplName(this string fullname)
        {
            var emptyspaceindex = fullname.Trim().IndexOf(' ');
            (string FirstName, string LastName) tplName;

            tplName.FirstName = (emptyspaceindex > -1) ? fullname.Trim().Substring(0, emptyspaceindex) : fullname;

            tplName.LastName = (emptyspaceindex > -1) ? fullname.Trim().Substring(emptyspaceindex).Trim() : string.Empty;

            return tplName;
        }
    }
}
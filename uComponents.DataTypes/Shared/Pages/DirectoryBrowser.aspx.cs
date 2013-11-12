namespace uComponents.DataTypes.Shared.Pages
{
    using umbraco.BusinessLogic;

    public class DirectoryBrowser : Umbraco.Web.UI.Umbraco.Developer.Packages.DirectoryBrowser
    {
        public DirectoryBrowser()
        {
            base.CurrentApp = "";
        }
    }
}

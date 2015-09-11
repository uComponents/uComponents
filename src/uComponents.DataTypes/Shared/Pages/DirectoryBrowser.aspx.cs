namespace uComponents.DataTypes.Shared.Pages
{
    using umbraco.BusinessLogic;

	/// <summary>
	/// DirectoryBrowser class
	/// </summary>
    public class DirectoryBrowser : Umbraco.Web.UI.Umbraco.Developer.Packages.DirectoryBrowser
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="DirectoryBrowser"/> class.
		/// </summary>
        public DirectoryBrowser()
        {
            base.CurrentApp = "";
        }
    }
}

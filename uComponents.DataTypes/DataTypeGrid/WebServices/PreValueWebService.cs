// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 10.04.2011 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Script.Services;
using System.Web.Services;
using umbraco;

namespace uComponents.DataTypes.DataTypeGrid.WebServices
{
    using System;

    /// <summary>
    /// Web service for Prevalues.
    /// </summary>
    [ScriptService]
    [WebService(Namespace = "http://umbraco.org/ucomponents/datatypegrid/prevalue")]
    public class PreValueWebService : WebService
    {
        /// <summary>
        /// Reorders the prevalue.
        /// </summary>
        /// <param name="preValueId">The prevalue id.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        /// <exception cref="System.UnauthorizedAccessException">Thrown if the user is not logged in.</exception>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public bool ReorderPreValue(string preValueId, string sortOrder)
        {
            if (umbraco.BusinessLogic.User.GetCurrent() == null)
            {
                throw new UnauthorizedAccessException();
            }

            return uQuery.ReorderPreValue(int.Parse(preValueId), int.Parse(sortOrder));
        }
    }
}

using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using uComponents.Core.Shared;
using umbraco.BusinessLogic;
using umbraco.DataLayer;

namespace uComponents.Core.Modules
{
	/// <summary>
	/// This class is used for JSON service that provides additional information about documents such as type, path etc.
	/// </summary>
	public class ItemInfoService
	{
		/// <summary>
		/// Gets the service path.
		/// </summary>
		/// <value>The service path.</value>
		public static string ServicePath
		{
			get
			{
				return ConfigurationManager.AppSettings["uComponentsItemInfoService"] ??
					(ConfigurationManager.AppSettings["umbracoPath"].Replace("~/", "/") + "/uComponents/Items");
			}
		}

		/// <summary>
		/// If the requested path matches the service path the request is processed and no further processing happens.
		/// A service request consists of a method name after the service path with the parameters provided as a JSON object.
		/// These methods are available
		///     "children": Returns the children of the parent specified as { parentID: [id of parent] }
		///     "range": Returns the items with the IDs specified as { ids: [array of ids] }
		/// The response includes:
		///     {
		///         id, path, uniqueID, text, typeAlias
		///     }
		///     
		/// </summary>
		/// <param name="app">The current HttpApplication instance</param>
		/// <returns></returns>
		public static bool ProcessRequest(HttpApplication app)
		{
			string path = app.Context.Request.Url.PathAndQuery;
			if (path.StartsWith(ServicePath))
			{
				var method = path.Substring(ServicePath.Length + 1).ToLower();
				var json = new JavaScriptSerializer();
				var nodes = new List<Dictionary<string, object>>();
				var sqlHelper = Application.SqlHelper;
				var sql = new StringBuilder();
				var ps = new List<IParameter>();

				Dictionary<string, object> request;
				using (var sr = new StreamReader(app.Request.InputStream, Encoding.UTF8))
				{
					request = (Dictionary<string, object>)json.DeserializeObject(sr.ReadToEnd());
				}

				if (method == "children" && request.ContainsKey("parentID"))
				{
					sql.Append(@"SELECT n.id, n.path, n.uniqueID, n.text, ct.alias as typeAlias
						FROM umbracoNode n
							INNER JOIN cmsContent c ON c.nodeId = n.id
							INNER JOIN cmsContentType ct ON ct.nodeId = c.contentType
							INNER JOIN umbracoNode ctn on ctn.id = ct.nodeId
						WHERE n.parentID = @parentID");
					ps.Add(sqlHelper.CreateParameter("@parentID", request["parentID"]));
				}
				else if (method == "range" && request.ContainsKey("ids"))
				{
					// check that the Ids are an array
					if (request["ids"] != null && typeof(object[]) == request["ids"].GetType())
					{
						int id;
						var ids = (object[])request["ids"];
						var nodeIds = new List<int>();

						// loop through array, making sure they are integers
						for (int i = 0; i < ids.Length; i++)
						{
							if (int.TryParse(ids[i].ToString(), out id))
							{
								nodeIds.Add(id);
							}
						}

						if (nodeIds.Count > 0)
						{
							sql.Append(@"SELECT n.id, n.path, n.uniqueID, n.text, ct.alias as typeAlias
								FROM umbracoNode n
									INNER JOIN cmsContent c ON c.nodeId = n.id
									INNER JOIN cmsContentType ct ON ct.nodeId = c.contentType
									INNER JOIN umbracoNode ctn on ctn.id = ct.nodeId
								WHERE n.id IN (");

							for (int i = 0; i < nodeIds.Count; i++)
							{
								var param = string.Concat("@p", i);
								if (i > 0)
								{
									sql.Append(Constants.Common.COMMA);
								}
								sql.Append(param);
								ps.Add(sqlHelper.CreateParameter(param, nodeIds[i]));
							}

							sql.Append(")");
						}
					}
				}

				if (sql.Length > 0)
				{
					var fields = new[] { "id", "path", "uniqueID", "text", "typeAlias" };

					using (var dr = sqlHelper.ExecuteReader(sql.ToString(), ps.ToArray()))
					{
						while (dr.Read())
						{
							var node = new Dictionary<string, object>();
							foreach (var field in fields)
							{
								node.Add(field, dr.GetObject(field));
							}
							nodes.Add(node);
						}
					}

					//// foreach (var node in nodes)
					//// {
					//// 	node["niceUrl"] = umbraco.library.NiceUrl(int.Parse("" + node["id"]));
					//// }
				}

				app.CompleteRequest();

				var response = Encoding.UTF8.GetBytes(json.Serialize(nodes));
				app.Response.ContentType = "application/json; charset=utf-8";
				app.Response.AddHeader("Content-Length", response.Length.ToString());
				app.Response.BinaryWrite(response);

				return true;
			}


			return false;
		}
	}
}
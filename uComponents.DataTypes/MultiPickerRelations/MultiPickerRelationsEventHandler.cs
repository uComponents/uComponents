using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using uComponents.Core;

using umbraco.BusinessLogic; // ApplicationBase
using umbraco.cms.businesslogic; // SaveEventArgs
using umbraco.cms.businesslogic.media; // Media
using umbraco.cms.businesslogic.member; // Member
using umbraco.cms.businesslogic.web; // Documentusing umbraco.cms.businesslogic.propertytype;
using umbraco.cms.businesslogic.property;
using umbraco.cms.businesslogic.relation;
using umbraco.DataLayer;

namespace uComponents.DataTypes.MultiPickerRelations
{
	/// <summary>
	/// Event handler that will convert a CSV into Relations
	/// </summary>
	public class MultiPickerRelationsEventHandler : ApplicationBase
	{
		//private enum MultiPickerStorageFormat
		//{
		//    Csv,
		//    Xml
		//}

		/// <summary>
		/// Initializes a new instance of MultiPickerRelationsEventHandler,
		/// hooks into the after event of saving a Content node, Media item or a Member
		/// </summary>
		public MultiPickerRelationsEventHandler()
		{
			Document.AfterSave += new Document.SaveEventHandler(this.AfterSave);
			Media.AfterSave += new Media.SaveEventHandler(this.AfterSave);
			Member.AfterSave += new Member.SaveEventHandler(this.AfterSave);

			Document.BeforeDelete += new Document.DeleteEventHandler(this.BeforeDelete);
			Media.BeforeDelete += new Media.DeleteEventHandler(this.BeforeDelete);
			Member.BeforeDelete += new Member.DeleteEventHandler(this.BeforeDelete);
		}


		/// <summary>
		/// Event after all properties have been saved
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AfterSave(Content sender, SaveEventArgs e)
		{
			var multiPickerRelationsId  = new Guid(DataTypeConstants.MultiPickerRelationsId);

			// For each MultiPickerRelations datatype
			foreach (Property multiPickerRelationsProperty in from property in sender.GenericProperties
															  where property.PropertyType.DataTypeDefinition.DataType.Id == multiPickerRelationsId
															  select property)
			{
				// used to identify this datatype instance - relations created are marked with this in the comment field
				string instanceIdentifier = "[\"PropertyTypeId\":" + multiPickerRelationsProperty.PropertyType.Id.ToString() + "]";

				// get configuration options for datatype
				MultiPickerRelationsOptions options = ((MultiPickerRelationsPreValueEditor)multiPickerRelationsProperty.PropertyType.DataTypeDefinition.DataType.PrevalueEditor).Options;

				// find MultiPicker source propertyAlias field on sender
				Property multiNodePickerProperty = sender.getProperty(options.PropertyAlias);

				if (multiNodePickerProperty != null)
				{
					// get relationType from options
					RelationType relationType = RelationType.GetById(options.RelationTypeId);

					if (relationType != null)
					{
						// validate: 1) check current type of sender matches that expected by the relationType, validation method is in the DataEditor
						uQuery.UmbracoObjectType contextObjectType = uQuery.UmbracoObjectType.Unknown;
						switch (sender.GetType().ToString())
						{
							case "umbraco.cms.businesslogic.web.Document": contextObjectType = uQuery.UmbracoObjectType.Document; break;
							case "umbraco.cms.businesslogic.media.Media": contextObjectType = uQuery.UmbracoObjectType.Media; break;
							case "umbraco.cms.businesslogic.member.Member": contextObjectType = uQuery.UmbracoObjectType.Member; break;
						}

						if (((MultiPickerRelationsDataEditor)multiPickerRelationsProperty.PropertyType.DataTypeDefinition.DataType.DataEditor)
							.IsContextUmbracoObjectTypeValid(contextObjectType, relationType))
						{

							uQuery.UmbracoObjectType pickerUmbracoObjectType = uQuery.UmbracoObjectType.Unknown;

							// Get the object type expected by the associated relation type and if this datatype has been configures as a rever index
							pickerUmbracoObjectType = ((MultiPickerRelationsDataEditor)multiPickerRelationsProperty.PropertyType.DataTypeDefinition.DataType.DataEditor)
														.GetPickerUmbracoObjectType(relationType);


							// clear all exisitng relations (or look to see previous verion of sender to delete changes ?)
							DeleteRelations(relationType, sender.Id, options.ReverseIndexing, instanceIdentifier);

							string multiPickerPropertyValue = multiNodePickerProperty.Value.ToString();

							var multiPickerStorageFormat = Settings.OutputFormat.CSV; // Assume default of csv

							if (uQuery.Helper.Xml.CouldItBeXml(multiPickerPropertyValue))
							{
								multiPickerStorageFormat = Settings.OutputFormat.XML;
							}

							// Creating instances of Documents / Media / Members ensures the IDs are of a valid type - be quicker to check with GetUmbracoObjectType(int)
							Dictionary<int, string> pickerNodes = null;
							switch (pickerUmbracoObjectType)
							{
								case uQuery.UmbracoObjectType.Document:
									switch (multiPickerStorageFormat)
									{
										case Settings.OutputFormat.CSV:
											pickerNodes = uQuery.GetDocumentsByCsv(multiPickerPropertyValue).ToNameIds();
											break;
										case Settings.OutputFormat.XML:
											pickerNodes = uQuery.GetDocumentsByXml(multiPickerPropertyValue).ToNameIds();
											break;
									}

									break;
								case uQuery.UmbracoObjectType.Media:
									switch (multiPickerStorageFormat)
									{
										case Settings.OutputFormat.CSV:
											pickerNodes = uQuery.GetMediaByCsv(multiPickerPropertyValue).ToNameIds();
											break;
										case Settings.OutputFormat.XML:
											pickerNodes = uQuery.GetMediaByXml(multiPickerPropertyValue).ToNameIds();
											break;
									}
									break;
								case uQuery.UmbracoObjectType.Member:
									switch (multiPickerStorageFormat)
									{
										case Settings.OutputFormat.CSV:
											pickerNodes = uQuery.GetMembersByCsv(multiPickerPropertyValue).ToNameIds();
											break;
										case Settings.OutputFormat.XML:
											pickerNodes = uQuery.GetMembersByXml(multiPickerPropertyValue).ToNameIds();
											break;
									}
									break;
							}
							if (pickerNodes != null)
							{
								foreach (KeyValuePair<int, string> pickerNode in pickerNodes)
								{
									CreateRelation(relationType, sender.Id, pickerNode.Key, options.ReverseIndexing, instanceIdentifier);
								}
							}
						}
						else
						{
							// Error: content object type invalid with relation type
						}
					}
					else
					{
						// Error: relation type is null
					}
				}
				else
				{
					// Error: multiNodePickerProperty alias not found
				}
			}
		}

		/// <summary>
		/// Clears any existing relations when deleting a node with a MultiPickerRelations datatype
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="umbraco.cms.businesslogic.DeleteEventArgs"/> instance containing the event data.</param>
		private void BeforeDelete(Content sender, DeleteEventArgs e)
		{
			var multiPickerRelationsId = new Guid(DataTypeConstants.MultiPickerRelationsId);

			// Clean up any relations

			// For each MultiPickerRelations datatype
			foreach (Property multiPickerRelationsProperty in from property in sender.GenericProperties
															  where property.PropertyType.DataTypeDefinition.DataType.Id == multiPickerRelationsId
															  select property)
			{
				// used to identify this datatype instance - relations created are marked with this in the comment field
				string instanceIdentifier = "[\"PropertyTypeId\":" + multiPickerRelationsProperty.PropertyType.Id.ToString() + "]";

				// get configuration options for datatype
				MultiPickerRelationsOptions options = ((MultiPickerRelationsPreValueEditor)multiPickerRelationsProperty.PropertyType.DataTypeDefinition.DataType.PrevalueEditor).Options;

				// get relationType from options
				RelationType relationType = RelationType.GetById(options.RelationTypeId);

				if (relationType != null)
				{
					// clear all exisitng relations
					DeleteRelations(relationType, sender.Id, options.ReverseIndexing, instanceIdentifier);
				}
			}
		}

		/// <summary>
		/// Delete all relations using the content node for a given RelationType
		/// </summary>
		/// <param name="relationType"></param>
		/// <param name="contentNodeId"></param>
		/// <param name="reverseIndexing"></param>
		/// <param name="instanceIdentifier">NOT USED ATM</param>
		private static void DeleteRelations(RelationType relationType, int contentNodeId, bool reverseIndexing, string instanceIdentifier)
		{
			//if relationType is bi-directional or a reverse index then we can't get at the relations via the API, so using SQL
			string getRelationsSql = "SELECT id FROM umbracoRelation WHERE relType = " + relationType.Id.ToString() + " AND ";

			if (reverseIndexing || relationType.Dual)
			{
				getRelationsSql += "childId = " + contentNodeId.ToString();
			}
			if (relationType.Dual) // need to return relations where content node id is used on both sides
			{
				getRelationsSql += " OR ";
			}
			if (!reverseIndexing || relationType.Dual)
			{
				getRelationsSql += "parentId = " + contentNodeId.ToString();
			}

			getRelationsSql += " AND comment = '" + instanceIdentifier + "'";

			using (IRecordsReader relations = uQuery.SqlHelper.ExecuteReader(getRelationsSql))
			{
				//clear data
				Relation relation;
				if (relations.HasRecords)
				{
					while (relations.Read())
					{
						relation = new Relation(relations.GetInt("id"));

						// TODO: [HR] check to see if an instance identifier is used
						relation.Delete();
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="relationType"></param>
		/// <param name="contentNodeId">id sourced from the Content / Media / Member</param>
		/// <param name="pickerNodeId">id sourced from the MultiPicker</param>
		/// <param name="reverseIndexing">if true, reverses the parentId and child Id</param>
		/// <param name="instanceIdentifier">JSON string with id of MultiPicker Relations property instance</param>
		private static void CreateRelation(RelationType relationType, int contentNodeId, int pickerNodeId, bool reverseIndexing, string instanceIdentifier)
		{
			if (reverseIndexing)
			{
				Relation.MakeNew(pickerNodeId, contentNodeId, relationType, instanceIdentifier);
			}
			else
			{
				Relation.MakeNew(contentNodeId, pickerNodeId, relationType, instanceIdentifier);
			}
		}
	}
}

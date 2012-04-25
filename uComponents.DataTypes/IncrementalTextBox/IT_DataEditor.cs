using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using uComponents.Core;
using uComponents.DataTypes.Shared.Extensions;
using umbraco.cms.businesslogic.datatype;

[assembly: WebResource("uComponents.DataTypes.IncrementalTextBox.Styles.IncrementalTextBox.css", Constants.MediaTypeNames.Text.Css, PerformSubstitution = true)]
[assembly: WebResource("uComponents.DataTypes.IncrementalTextBox.Scripts.jquery.alphanumeric.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.DataTypes.IncrementalTextBox.Scripts.jquery.increment.js", Constants.MediaTypeNames.Application.JavaScript)]
[assembly: WebResource("uComponents.DataTypes.IncrementalTextBox.Images.down.png", Constants.MediaTypeNames.Image.Png)]
[assembly: WebResource("uComponents.DataTypes.IncrementalTextBox.Images.up.png", Constants.MediaTypeNames.Image.Png)]

namespace uComponents.DataTypes.IncrementalTextBox
{
	/// <summary>
	/// The DataEditor for the IncrementalTextBox data-type.
	/// </summary>
    public class IT_DataEditor : CompositeControl 
    {
		/// <summary>
		/// The TextBox control for the value.
		/// </summary>
        protected TextBox TextBoxValue;
		
		/// <summary>
		/// Gets or sets the m_prevalues.
		/// </summary>
		/// <value>The m_prevalues.</value>
        public IT_PrevalueEditor m_prevalues { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		public string Text
        {
            get
            {
                return this.TextBoxValue.Text;
            }
            set
            {
                if (this.TextBoxValue == null)
                {
                    this.TextBoxValue = new TextBox();
                    this.TextBoxValue.ID = this.ID + "TextBox";
                }

                this.TextBoxValue.Text = value;
            }
        }

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.EnsureChildControls();
        }

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

			// get the urls for the embedded resources
			this.AddResourceToClientDependency("uComponents.DataTypes.IncrementalTextBox.Scripts.jquery.increment.js", ClientDependency.Core.ClientDependencyType.Javascript);
			this.AddResourceToClientDependency("uComponents.DataTypes.IncrementalTextBox.Scripts.jquery.alphanumeric.js", ClientDependency.Core.ClientDependencyType.Javascript);
			this.AddResourceToClientDependency("uComponents.DataTypes.IncrementalTextBox.Styles.IncrementalTextBox.css", ClientDependency.Core.ClientDependencyType.Css);
        }

		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            this.EnsureChildControls();

            this.Controls.Add(TextBoxValue);
        }

		/// <summary>
		/// Writes the <see cref="T:System.Web.UI.WebControls.CompositeControl"/> content to the specified <see cref="T:System.Web.UI.HtmlTextWriter"/> object, for display on the client.
		/// </summary>
		/// <param name="writer">An <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            TextBoxValue.RenderControl(writer);
            
            writer.AddAttribute("onclick", "{0}_increment()".Replace("{0}", TextBoxValue.ClientID));            
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "IncrementTextBoxUp");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();

            writer.AddAttribute("onclick", "{0}_decrement()".Replace("{0}", TextBoxValue.ClientID));            
            writer.AddAttribute(HtmlTextWriterAttribute.Class,"IncrementTextBoxDown" );
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();

            string javascript = @"
            <script type='text/javascript'>
                    jQuery('#{0}').numeric();

                    function {0}_increment() {
						var value = parseInt(jQuery('#{0}').val()) + {3};
                        if ( value < {1} ) {
							jQuery('#{0}').val({1} + {3});
						} else if ( value <= {2} ) {
                            jQuery('#{0}').increment({3});
						} else if ( value > {2} ) {
							jQuery('#{0}').val({2});
						}
                    }
                    function {0}_decrement() {
						var value = parseInt(jQuery('#{0}').val()) - {3};
						if ( value >= {2} ) {
							jQuery('#{0}').val({2} - {3});
                        } else if ( value >= {1} && value <= {2} ) {
                            jQuery('#{0}').decrement(-{3});
						} else {
							jQuery('#{0}').val({1});
						}
                    }                                        
            </script>

".Replace("{0}", TextBoxValue.ClientID).Replace("{1}", m_prevalues.MinValue.ToString()).Replace("{2}", m_prevalues.MaxValue.ToString()).Replace("{3}",m_prevalues.IncrementValue.ToString());

            writer.WriteLine(javascript);
        }
    }
}

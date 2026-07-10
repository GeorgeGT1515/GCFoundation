using GCFoundation.Common.Utilities;
using GCFoundation.Components.Enums;
using GCFoundation.Components.Models.Table;
using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// Renders a data table using the gcds-table element.
    /// Use &lt;fdcp-table&gt; in your Razor views to generate a table, either by binding row models
    /// via <c>from</c>, or by supplying explicit <c>columns</c>/<c>data</c> JSON.
    /// </summary>
    [HtmlTargetElement("fdcp-table", Attributes = "for, caption")]
    [HtmlTargetElement("fdcp-table", Attributes = "caption, cols, rows")]
    [HtmlTargetElement("fdcp-table", Attributes="caption, columns, data")]
    public class FDCPTableTagHelper : TableTagHelper
    {
        private string? _templatesHtml { get; set; }
        /// <summary>
        /// The collection of row data models to render in the table. Column definitions are generated
        /// automatically by reflecting over the properties of the model type.
        /// </summary>
        public ModelExpression? For { get; set; }

        public ModelExpression? Cols { get; set; }
        public ModelExpression? Rows { get; set; }

        /// <summary>
        /// The accessible name given to the table via the <c>caption</c> slot, so assistive technologies
        /// can identify and announce it. Rendered as the table's caption heading.
        /// </summary>
        public string? Caption { get; set; }

        /// <summary>
        /// Additional detail text shown under the caption heading, providing further context about the table.
        /// </summary>
        public string? CaptionDetail { get; set; }

        /// <summary>
        /// Whether to mark each cell in the first column as a row header. Row headers label what each row is about.
        /// </summary>
        [HtmlAttributeName("rowHeader")]
        public bool? RowHeader { get; set; }

        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output, nameof(output));

            if (For != null)
                BuildFromRows();            

            if (Cols != null && Rows != null)
            {
                BuildFromColsAndRows();
            }

            output.TagName = "gcds-table";
            output.TagMode = TagMode.StartTagAndEndTag;

            base.Process(context, output);
            output.PreContent.SetHtmlContent(BuildHtml());
            output.PostContent.SetHtmlContent(_templatesHtml);
        }

        #region BuildColumnsAndData
        private void BuildFromRows()
        {
            Type dataType;
            IEnumerable<object> data;

            ResolveModelExpression(For!, out dataType, out data);

            Data = JsonSerializer.Serialize(data, JsonOptionsUtility.CamelCase);

            List<TableColumnModel> columns = new List<TableColumnModel>();
            var properties = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var column = new TableColumnModel() 
                {
                  Field = JsonNamingPolicy.CamelCase.ConvertName(property.Name), 
                  Header = ResolveLocalizedHeader(property), 
                  RowHeader = false 
                };
                columns.Add(column);
            }

            if (RowHeader == true)
                columns[0].RowHeader = true;

            Columns = JsonSerializer.Serialize(columns, JsonOptionsUtility.CamelCase);
        }

        private void BuildFromColsAndRows()
        {
            Type columnsType;
            IEnumerable<object> columns;

            ResolveModelExpression(Cols!, out columnsType, out columns);
            var properties = columnsType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (!(properties.Any(p => p.Name == "Field") && properties.Any(p => p.Name == "Header")))
                return;

            Columns = JsonSerializer.Serialize(columns, JsonOptionsUtility.CamelCase);
            Data = JsonSerializer.Serialize(Rows.Model!, JsonOptionsUtility.CamelCase);

            var slottedProperty = properties.FirstOrDefault(p => p.Name == "Slotted");
            if (slottedProperty is not null)
            {
                var fieldProperty = properties.First(p => p.Name == "Field"); // looked up ONCE

                var slottedColumns = columns
                    .Where(c => slottedProperty.GetValue(c) is bool b && b)
                    .ToList();

                if (slottedColumns.Count > 0)
                {
                    _templatesHtml = BuildTemplates(slottedColumns, columnsType, fieldProperty);
                }
            }
        }
        #endregion

        #region BuildHtmlContent
        private string BuildHtml()
        {
            string captionDetailHtml = string.IsNullOrEmpty(CaptionDetail) ? string.Empty : $"<p>{CaptionDetail}</p>";
            string html = $"""
                <div slot="caption">
                    <h5>{Caption}</h5>
                    {captionDetailHtml}
                </div>
                """;
            return html;
        }

        private static string BuildTemplates(List<object> slottedColumns, Type columnsType, PropertyInfo fieldProperty)
        {
            var slotTypeProperty = columnsType.GetProperty("SlotType");
            var hrefTemplateProperty = columnsType.GetProperty("SlotHrefTemplate");
            var displayFieldProperty = columnsType.GetProperty("SlotDisplayField");
            var buttonLabelProperty = columnsType.GetProperty("SlotButtonLabel");
            var actionNameProperty = columnsType.GetProperty("SlotActionName");

            if (slotTypeProperty is null)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (var column in slottedColumns)
            {
                var field = (string)fieldProperty.GetValue(column)!;
                var slotType = slotTypeProperty.GetValue(column) as SlotType?;

                string template = slotType switch
                {
                    SlotType.Link => BuildLinkTemplate(field, column, hrefTemplateProperty, displayFieldProperty),
                    SlotType.Button => BuildButtonTemplate(field, buttonLabelProperty?.GetValue(column) as string, actionNameProperty?.GetValue(column) as string),
                    _ => string.Empty
                };

                if (!string.IsNullOrEmpty(template))
                    sb.Append(template);
            }

            return sb.ToString();
        }

        private static string BuildLinkTemplate(string field, object column, PropertyInfo? hrefTemplateProperty, PropertyInfo? displayFieldProperty)
        {
            var href = hrefTemplateProperty?.GetValue(column);
            var displayField = displayFieldProperty?.GetValue(column) ?? field;

            return $"""
                <template slot="cell:{field}">
                    <a data-bind-template-href="{href}" data-bind="{displayField}"></a>
                </template>
                """;
        }

        private static string BuildButtonTemplate(string field, string? label, string? actionName)
        {
            var actionAttr = string.IsNullOrEmpty(actionName) ? string.Empty : $" data-action=\"{actionName}\"";

            if (!string.IsNullOrEmpty(label))
            {
                return $"""
                    <template slot="cell:{field}">
                        <gcds-button button-role="secondary" size="small"{actionAttr}>{label}</gcds-button>
                    </template>
                    """;
            }

            return $"""
                <template slot="cell:{field}">
                    <gcds-button button-role="secondary" size="small"{actionAttr} data-bind="{field}"></gcds-button>
                </template>
                """;
        }
        #endregion

        #region Resolvers
        private static string ResolveLocalizedHeader(PropertyInfo property)
        {
            ArgumentNullException.ThrowIfNull(property, nameof(property));

            var displayAttr = property.GetCustomAttribute<DisplayAttribute>();
            return displayAttr?.GetName() ?? property.Name;
        }

        private static void ResolveModelExpression(ModelExpression modelExpression, out Type type, out IEnumerable<object> items)
        {
            ArgumentNullException.ThrowIfNull(modelExpression, nameof(modelExpression));

            if (modelExpression.Metadata.IsCollectionType)
            {
                type = modelExpression.Metadata.ElementType!;
                items = ((System.Collections.IEnumerable)modelExpression.Model!).Cast<object>();
            }
            else
            {
                type = modelExpression.Metadata.ModelType;
                items = new[] { modelExpression.Model! };
            }
        }
        #endregion
    }
}


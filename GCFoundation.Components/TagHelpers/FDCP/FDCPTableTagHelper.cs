using GCFoundation.Common.Utilities;
using GCFoundation.Components.Attributes.Table;
using GCFoundation.Components.Enums;
using GCFoundation.Components.Models.TableBuilder;
using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Text.Json;


namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// Renders a data table using the gcds-table element.
    /// Use &lt;fdcp-table&gt; in your Razor views to generate a table, either by binding row models
    /// via <c>from</c>, or by supplying explicit <c>columns</c>/<c>data</c> JSON.
    /// </summary>
    [HtmlTargetElement("fdcp-table", Attributes = "caption, rows")]
    [HtmlTargetElement("fdcp-table", Attributes = "caption, column-definitions, rows")]
    public class FDCPTableTagHelper : TableTagHelper
    {
        /// <summary>
        /// The column definitions for the table. If <c>null</c> or empty, columns are resolved
        /// automatically from the properties of the row model in <see cref="Rows"/>.
        /// </summary>
        public ICollection<ColumnDefinition>? ColumnDefinitions { get; set; }

        /// <summary>
        /// The row data to render in the table. Each element represents one row. If
        /// <see cref="ColumnDefinitions"/> is provided, its properties supply the cell values matched
        /// by <see cref="ColumnDefinition.Field"/>; otherwise, its properties are also used to resolve
        /// the columns themselves.
        /// </summary>
        public IEnumerable<Object>? Rows { get; set; }

        /// <summary>
        /// The accessible name given to the table via the <c>caption</c> slot, so assistive technologies
        /// can identify and announce it. Rendered as the table's caption heading.
        /// </summary>
        public string? Caption { get; set; }

        /// <summary>
        /// Additional detail text shown under the caption heading, providing further context about the table.
        /// </summary>
        public string? CaptionDetail { get; set; }

        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output, nameof(output));

            if (ColumnDefinitions == null)
                BuildFromRows();

            if (ColumnDefinitions != null && Rows != null)
            {
                BuildFromColsAndRows();
            }
            
            output.TagName = "gcds-table";
            output.TagMode = TagMode.StartTagAndEndTag;

            base.Process(context, output);
            output.PreContent.SetHtmlContent(BuildHtml());
            //output.PostContent.SetHtmlContent(_templatesHtml);
        }

        #region BuildColumnsAndData
        private void BuildFromRows()
        {
            ResolveColumns();
            BuildFromColsAndRows();
        }

        private void BuildFromColsAndRows()
        {
            Columns = JsonSerializer.Serialize(ColumnDefinitions, JsonOptionsUtility.CamelCaseIgnoreNull);
            Data = JsonSerializer.Serialize(Rows!, JsonOptionsUtility.CamelCase);

            //var properties = columnsType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //var slottedProperty = properties.FirstOrDefault(p => p.Name == "Slotted");
            //if (slottedProperty is not null)
            //{
            //    var fieldProperty = properties.First(p => p.Name == "Field"); // looked up ONCE

            //    var slottedColumns = columns
            //        .Where(c => slottedProperty.GetValue(c) is bool b && b)
            //        .ToList();

            //    if (slottedColumns.Count > 0)
            //    {
            //        _templatesHtml = BuildTemplates(slottedColumns, columnsType, fieldProperty);
            //    }
            //}
        }
        #endregion

        #region BuildHtmlContent
        private string BuildHtml()
        {
            string captionDetailHtml = string.IsNullOrEmpty(CaptionDetail) ? string.Empty : $"<gcds-text>{CaptionDetail}</gcds-text>";
            string html = $"""
                <div slot="caption">
                    <gcds-heading tag="h5">{Caption}</gcds-heading>
                    {captionDetailHtml}
                </div>
                """;
            return html;
        }

        private static string BuildTemplates(List<object> slottedColumns, Type columnsType, PropertyInfo fieldProperty)
        {
            var slotTypeProperty = columnsType.GetProperty("SlotType");
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
                    SlotType.link => BuildButtonTemplate(field, buttonLabelProperty?.GetValue(column) as string, actionNameProperty?.GetValue(column) as string),
                    SlotType.button => BuildButtonTemplate(field, buttonLabelProperty?.GetValue(column) as string, actionNameProperty?.GetValue(column) as string),
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
        private void ResolveColumns()
        {
            if (Rows != null && Rows.Any())
            {
                Type type = Rows.First().GetType();
                var properties = type != null ? type.GetProperties() : null;
                ColumnDefinitions = new List<ColumnDefinition>();
                if (properties == null)
                    return;
                foreach (PropertyInfo prop in properties)
                {
                    TableColumnDefinitionAttribute attribute = prop.GetCustomAttribute<TableColumnDefinitionAttribute>();
                    if (attribute != null)
                    {
                        if (!attribute.IsHidden)
                        {
                            ColumnDefinitions.Add(new ColumnDefinition()
                            {
                                Field = JsonNamingPolicy.CamelCase.ConvertName(prop.Name),
                                Header = ResolveLocalizedHeader(prop),
                                Slotted = attribute.Slotted,
                                RowHeader = attribute.RowHeader,
                                Sort = attribute.Sort,
                                SortDirection = attribute.SortDirection == SortDirection.None ? null : attribute.SortDirection,
                                Alignment = attribute.Alignment
                            });
                        }
                    }
                    else
                    {
                        ColumnDefinitions.Add(new ColumnDefinition()
                        {
                            Field = JsonNamingPolicy.CamelCase.ConvertName(prop.Name),
                            Header = ResolveLocalizedHeader(prop)
                        });
                    }
                }
            }

            return;

        }

        private static string ResolveLocalizedHeader(PropertyInfo property)
        {
            ArgumentNullException.ThrowIfNull(property, nameof(property));

            var displayAttr = property.GetCustomAttribute<DisplayAttribute>();
            return displayAttr?.GetName() ?? property.Name;
        }
        #endregion
    }
}
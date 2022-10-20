using System.Collections.Generic;
#if NET472
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public class TemplatedItemPickerDataListEditor : IDataListEditor
    {
        private readonly IIOHelper _ioHelper;

        public TemplatedItemPickerDataListEditor(IIOHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }

        public string Name => "Templated Item Picker";

        public string Description => "Select items from an Umbraco style overlay.";

        public string Icon => "icon-fa fa-mouse-pointer";

        public string Group => default;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new NotesConfigurationField(_ioHelper, @"<details class=""well well-small"">
<summary><strong>Do you need help with your custom template?</strong></summary>
<p>Your custom template will be used to display an individual item from your configured data source.</p>
<p>The data for the item will be in the following format:</p>
<pre><code>{
  ""name"": ""..."",
  ""value"": ""..."",
  ""description"": ""..."", // optional
  ""icon"": ""..."", // optional
  ""disabled"": ""true|false"", // optional
  ""selected"": ""true|false"",
}</code></pre>
<p>If you are familiar with AngularJS template syntax, you can display the values using an expression: e.g. <code ng-non-bindable>{{ item.name }}</code>.</p>
<p>If you need assistance with AngularJS expression syntax, please refer to this resource: <a href=""https://docs.angularjs.org/guide/expression"" target=""_blank""><strong>docs.angularjs.org</strong></a>.</p>
<hr>
<p>If you would like a starting point for your custom template, here is an example.</p>
<umb-code-snippet language=""'AngularJS template'"">&lt;i class=""icon"" ng-class=""item.icon""&gt;&lt;/i&gt;
&lt;span ng-bind=""item.name""&gt;&lt;/span&gt;</umb-code-snippet>
</details>", true),
            new ConfigurationField
            {
                Key = "template",
                Name = "Template",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(CodeEditorDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { CodeEditorConfigurationEditor.Mode, "razor" },
                    { "minLines", 12 },
                    { "maxLines", 30 },
                }
            },
            new ConfigurationField
            {
                Key = "overlaySize",
                Name = "Editor overlay size",
                Description = "Select the size of the overlay editing panel. By default this is set to 'small'. However if the editor fields require a wider panel, please select 'medium' or 'large'.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(RadioButtonListDataListEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "Small", Value = "small" },
                            new DataListItem { Name = "Medium", Value = "medium" },
                            new DataListItem { Name = "Large", Value = "large" }
                        }
                    },
                    { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, "small" }
                }
            },
            new ConfigurationField
            {
                Key = "defaultIcon",
                Name = "Default icon",
                Description = "Select an icon to be displayed as the default icon,<br><em>(for when no icon is available)</em>.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl("~/umbraco/views/propertyeditors/listview/icon.prevalues.html"),
            },
            new ConfigurationField
            {
                Key = "listType",
                Name = "List type",
                Description = "Select the style of list to be displayed in the overlay.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(RadioButtonListDataListEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "Grid", Value = "grid", Description = "Displays as a card based layout, (3 cards per row)." },
                            new DataListItem { Name = "List", Value = "list", Description = "Displays as a single column menu, (with descriptions, if available)." }
                        }
                    },
                    { ShowDescriptionsConfigurationField.ShowDescriptions, Constants.Values.True },
                }
            },
            new EnableFilterConfigurationField
            {
                View = "views/propertyeditors/boolean/boolean.html",
                Config = new Dictionary<string, object>
                {
                    { "default", Constants.Values.True }
                },
            },
            new MaxItemsConfigurationField(_ioHelper),
            new AllowClearConfigurationField(),
            new ConfigurationField
            {
                Key = "allowDuplicates",
                Name = "Allow duplicates?",
                Description = "Select to allow the editor to select duplicate items.",
                View = "boolean",
            },
            new ConfigurationField
            {
                Key = "enableMultiple",
                Name = "Multiple selection?",
                Description = "Select to enable picking multiple items.",
                View = "boolean",
            },
            new DisableSortingConfigurationField(),
            new ConfigurationField
            {
                Key ="confirmRemoval",
                Name = "Confirm removals?",
                Description = "Select to enable a confirmation prompt when removing an item.",
                View = "boolean",
            }
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>()
        {
            { "listType", "list" },
            { "displayMode", "template" },
            { "defaultIcon", UmbConstants.Icons.DefaultIcon },
            { EnableFilterConfigurationField.EnableFilter, Constants.Values.True },
            { MaxItemsConfigurationField.MaxItems, "0" },
        };

        public Dictionary<string, object> DefaultConfig => new Dictionary<string, object>()
        {
            { Constants.Conventions.ConfigurationFieldAliases.OverlayView, _ioHelper.ResolveRelativeOrVirtualUrl(ItemPickerDataListEditor.DataEditorOverlayViewPath) },
            { "overlayOrderBy", string.Empty },
        };

        public bool HasMultipleValues(Dictionary<string, object> config)
        {
            return config.TryGetValueAs(MaxItemsConfigurationField.MaxItems, out int maxItems) == true && maxItems != 1;
        }

        public OverlaySize OverlaySize => OverlaySize.Small;

        public string View => ItemPickerDataListEditor.DataEditorViewPath;
    }
}

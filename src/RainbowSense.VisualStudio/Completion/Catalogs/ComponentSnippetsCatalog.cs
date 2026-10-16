using System.Collections.Generic;

namespace RainbowSense.VisualStudio.Completion.Catalogs;

public sealed class ComponentSnippet
{
    public ComponentSnippet(string tagName, string skeleton, string documentation)
    {
        TagName = tagName;
        Skeleton = skeleton;
        Documentation = documentation;
    }

    public string TagName { get; }

    public string Skeleton { get; }

    public string Documentation { get; }
}

public static class ComponentSnippetsCatalog
{
    private static readonly IReadOnlyList<ComponentSnippet> _items = new List<ComponentSnippet>
    {
        new ComponentSnippet(
            "DxGrid",
            "<DxGrid DataSource=\"@DataSource\">$0\n    <Columns>\n        <DxGridDataColumn Field=\"\" />\n    </Columns>\n</DxGrid>",
            "Data grid skeleton with a data column."),
        new ComponentSnippet(
            "DxTreeList",
            "<DxTreeList DataSource=\"@DataSource\">$0\n    <Columns>\n        <DxTreeListColumn Field=\"\" />\n    </Columns>\n</DxTreeList>",
            "Hierarchical grid skeleton."),
        new ComponentSnippet(
            "DxFormLayout",
            "<DxFormLayout>$0\n    <DxFormLayoutItem Caption=\"\">\n        <DxTextBox />\n    </DxFormLayoutItem>\n</DxFormLayout>",
            "Form layout skeleton with a text editor."),
        new ComponentSnippet(
            "DxFormLayoutItem",
            "<DxFormLayoutItem Caption=\"\" ColSpan=\"1\">$0\n    <DxTextBox />\n</DxFormLayoutItem>",
            "Single form layout item."),
        new ComponentSnippet(
            "DxComboBox",
            "<DxComboBox @bind-Value=\"@Value\" DataSource=\"@Items\" />$0",
            "Combo box bound to a value."),
        new ComponentSnippet(
            "DxTextBox",
            "<DxTextBox @bind-Value=\"@Value\" />$0",
            "Text editor bound to a value."),
        new ComponentSnippet(
            "DxMemo",
            "<DxMemo @bind-Value=\"@Value\" Rows=\"4\" />$0",
            "Multiline text editor bound to a value."),
        new ComponentSnippet(
            "DxButton",
            "<DxButton Text=\"\" OnClick=\"@OnClick\">$0\n</DxButton>",
            "Button with a click handler."),
        new ComponentSnippet(
            "DxValidator",
            "<DxValidator>\n    <RequiredRule />$0\n    <NumericRule />\n</DxValidator>",
            "Validation rules for an editor."),
        new ComponentSnippet(
            "DxChart",
            "<DxChart>$0\n    <DxPlotArea>\n        <DxBarSeries />\n    </DxPlotArea>\n    <DxLegend />\n</DxChart>",
            "Chart skeleton with a bar series."),
        new ComponentSnippet(
            "DxPieChart",
            "<DxPieChart DataSource=\"@DataSource\">$0\n    <DxSeries>\n        <DxPieChartSeries ValueField=\"\" NameField=\"\" />\n    </DxSeries>\n</DxPieChart>",
            "Pie chart skeleton."),
        new ComponentSnippet(
            "DxScheduler",
            "<DxScheduler DataSource=\"@DataSource\" StartDate=\"@DateTime.Today\">$0\n    <DxAppointmentForm />\n</DxScheduler>",
            "Scheduler skeleton."),
        new ComponentSnippet(
            "DxTreeView",
            "<DxTreeView DataSource=\"@DataSource\">$0\n    <DxTreeViewNode Text=\"\" />\n</DxTreeView>",
            "Tree view skeleton."),
        new ComponentSnippet(
            "DxListView",
            "<DxListView DataSource=\"@DataSource\" SelectionMode=\"ListViewSelectionMode.Single\">$0\n</DxListView>",
            "List view skeleton."),
        new ComponentSnippet(
            "DxAccordion",
            "<DxAccordion>$0\n    <DxAccordionItem Text=\"\">\n    </DxAccordionItem>\n</DxAccordion>",
            "Accordion skeleton."),
        new ComponentSnippet(
            "DxTabLayout",
            "<DxTabLayout>$0\n    <DxTabLayoutItem Text=\"\" />\n</DxTabLayout>",
            "Tab layout skeleton."),
        new ComponentSnippet(
            "DxDrawer",
            "<DxDrawer Open=\"true\" Position=\"DrawerPosition.Left\">$0\n</DxDrawer>",
            "Drawer panel."),
        new ComponentSnippet(
            "DxPager",
            "<DxPager PageIndex=\"@PageIndex\" PageSize=\"10\" />$0",
            "Pager control."),
        new ComponentSnippet(
            "InputText",
            "<InputText @bind-Value=\"@Value\" />$0",
            "Text input bound to a value."),
        new ComponentSnippet(
            "InputNumber",
            "<InputNumber @bind-Value=\"@Value\" />$0",
            "Numeric input bound to a value."),
        new ComponentSnippet(
            "EditForm",
            "<EditForm Model=\"@Model\">$0\n    <DataAnnotationsValidator />\n    <ValidationSummary />\n</EditForm>",
            "Form with data annotations validation."),
        new ComponentSnippet(
            "AuthorizeView",
            "<AuthorizeView>$0\n    <Authorized>\n    </Authorized>\n    <NotAuthorized>\n    </NotAuthorized>\n</AuthorizeView>",
            "Content shown based on the authentication state."),
        new ComponentSnippet(
            "Virtualize",
            "<Virtualize Items=\"@Items\" Context=\"item\">$0\n    <div>@item</div>\n</Virtualize>",
            "Virtualized list."),
    };

    public static IReadOnlyList<ComponentSnippet> Items => _items;

    public static ComponentSnippet Find(string tagName)
    {
        foreach (ComponentSnippet snippet in _items)
        {
            if (string.Equals(snippet.TagName, tagName, System.StringComparison.Ordinal))
                return snippet;
        }

        return null;
    }
}
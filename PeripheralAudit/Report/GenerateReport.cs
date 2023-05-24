using HtmlAgilityPack;
using PeripheralAudit.Application;
using PeripheralAudit.Application.Entities;

namespace PeripheralAudit.Report;

public sealed class GenerateReport
{
    private PeripheralAuditDbContext _dbContext;
    private string _reportOutput;
    private Cost _costs;

    HtmlDocument _template = new HtmlDocument();

    public GenerateReport(PeripheralAuditDbContext dbContext, string reportOutput, Cost costs)
    {
        _dbContext = dbContext;
        _reportOutput = reportOutput;
        _costs = costs;
    }

    internal void Execute()
    {
        const string CSS_FILE = "AuditTemplate.css";
        const string HTML_FILE = "AuditTemplate.html";

        string cssTemplate = Path.Combine("./HtmlTemplate", CSS_FILE);
        string cssOutput = Path.Combine(_reportOutput, CSS_FILE);
        if (!File.Exists(cssOutput))
            File.Copy(cssTemplate, cssOutput);

        string htmlTemplate = Path.Combine("./HtmlTemplate", HTML_FILE);

        List<Site> sites = _dbContext.Sites.ToList();
        foreach (Site site in sites)
        {
            string htmlOutput = Path.Combine(_reportOutput, site.Name + ".html");
            if (!File.Exists(htmlOutput))
                File.Delete(htmlOutput);

            _template.Load(htmlTemplate);

            string reportDate = _template.GetElementbyId("reportDate").InnerHtml;

            HtmlNode reportDateDiv = _template.DocumentNode.SelectSingleNode("//div[@id='reportDate']");
            reportDateDiv.InnerHtml = $"Report produced: {DateTime.Now.ToLongDateString()}";

            HtmlNode reportSite = _template.DocumentNode.SelectSingleNode("//div[@id='reportSite']");
            reportSite.InnerHtml = site.Name;

            HtmlNode reportTable = _template.DocumentNode.SelectSingleNode("//table[@id='reportTable']");

            HtmlNode reportRow = _template.DocumentNode.SelectSingleNode("//tr[@id='reportHeader']");

            List<Location> locations = _dbContext.Locations
                .Where(l => l.Site.Id == site.Id)
                .OrderByDescending(l => l.Name)
                .ToList();

            foreach (Location location in locations)
            {
                HtmlNode[] newRow = ReportRow(location, _costs);
                if (newRow[1] is not null)
                    reportTable.InsertAfter(newRow[1], reportRow);
                reportTable.InsertAfter(newRow[0], reportRow);
            }

            _template.Save(htmlOutput);
        }
    }

    private HtmlNode[] ReportRow(Location location, Cost costs)
    {
        HtmlNode[] tr = new HtmlNode[2];

        tr[0] = _template.CreateElement("tr");
        HtmlNode name = TableData(location.Name, classAttribute: "ta1");
        tr[0].ChildNodes.Append(name);
        HtmlNode desks = TableData(location.DeskCount);
        tr[0].ChildNodes.Append(desks);
        HtmlNode single = TableData(location.MonitorSingleCount);
        tr[0].ChildNodes.Append(single);
        HtmlNode dual = TableData(location.MonitorDualCount);
        tr[0].ChildNodes.Append(dual);
        HtmlNode bronze = TableData(location.MonitorGradeBronzeCount);
        tr[0].ChildNodes.Append(bronze);
        HtmlNode silver = TableData(location.MonitorGradeSilverCount);
        tr[0].ChildNodes.Append(silver);
        HtmlNode gold = TableData(location.MonitorGradeGoldCount);
        tr[0].ChildNodes.Append(gold);
        HtmlNode dock = TableData(location.DockCount);
        tr[0].ChildNodes.Append(dock);
        HtmlNode pc = TableData(location.PcCount);
        tr[0].ChildNodes.Append(pc);
        HtmlNode keyboard = TableData(location.KeyboardCount);
        tr[0].ChildNodes.Append(keyboard);
        HtmlNode mouse = TableData(location.MouseCount);
        tr[0].ChildNodes.Append(mouse);
        HtmlNode chair = TableData(location.ChairCount);
        tr[0].ChildNodes.Append(chair);

        HtmlNode? repopultionCosts = UpgradeCosts(location, costs);
        if (repopultionCosts is not null)
        {
            tr[1] = _template.CreateElement("tr");
            HtmlNode noBorderNoBackground = TableData("", classAttribute: "nbnb");
            tr[1].ChildNodes.Append(noBorderNoBackground);

            tr[1].ChildNodes.Append(repopultionCosts);
        }

        return tr;
    }

    private HtmlNode TableData(int content)
    {
        return TableData(content.ToString());
    }

    private HtmlNode TableData(string content, int? colSpan = null, string? classAttribute = null, string? style = null)
    {
        HtmlNode tableData = _template.CreateElement("td");
        if (colSpan is not null)
            tableData.Attributes.Add("colspan", colSpan.ToString());
        if (classAttribute is not null)
            tableData.Attributes.Add("class", classAttribute);
        if (style is not null)
            tableData.Attributes.Add("style", style);
        tableData.InnerHtml = content;
        return tableData;
    }

    public HtmlNode? UpgradeCosts(Location location, Cost costs)
    {
        if (location.DeskCount == location.MonitorGradeGoldCount &&
            location.DeskCount == location.DockCount &&
            location.DeskCount == location.KeyboardCount &&
            location.DeskCount == location.MouseCount)
            return null;

        Upgrade upgrade = new(costs, location);

        HtmlNode upgradeData = _template.CreateElement("td");
        upgradeData.Attributes.Add("colspan", "12");
        upgradeData.Attributes.Add("class", "tal");

        string htmlBreak = string.Empty;

        if (upgrade.RepopulationCost > 0)
        {
            HtmlNode bronze = _template.CreateElement(nameof(bronze));
            bronze.InnerHtml = $"Repopultion Costs &#163;{upgrade.RepopulationCost} - ";
            upgradeData.AppendChild(bronze);

            HtmlNode dock = _template.CreateElement(nameof(dock));
            dock.InnerHtml = $"{upgrade.DockCount} dock(s) @ &#163;{_costs.Dock}, ";
            upgradeData.AppendChild(dock);

            HtmlNode monitor = _template.CreateElement(nameof(monitor));
            monitor.InnerHtml = $"{upgrade.BronzeMonitorCount} monitor(s) @ &#163;0, ";
            upgradeData.AppendChild(monitor);

            HtmlNode keyboard = _template.CreateElement(nameof(keyboard));
            keyboard.InnerHtml = $"{upgrade.KeyboardCount} keyboard(s) @ &#163;{_costs.Keyboard}, ";
            upgradeData.AppendChild(keyboard);

            HtmlNode mouse = _template.CreateElement(nameof(mouse));
            mouse.InnerHtml = $"{upgrade.MouseCount} mice @ &#163;{_costs.Mouse}";
            upgradeData.AppendChild(mouse);

            htmlBreak = "<br />";
        }

        if (upgrade.SilverMonitorCount > 0)
        {
            HtmlNode silver = _template.CreateElement(nameof(silver));
            silver.InnerHtml = $"{htmlBreak}Silver Upgrade Costs &#163;{_costs.Monitor} - {upgrade.SilverMonitorCount} monitor(s) @ &#163;{_costs.Monitor}";
            upgradeData.AppendChild(silver);

            htmlBreak = "<br />";
        }

        if (upgrade.GoldMonitorCount > 0)
        {
            HtmlNode gold = _template.CreateElement(nameof(gold));
            gold.InnerHtml = $"{htmlBreak}Gold Upgrade Costs &#163;{_costs.LargeMonitor} - {upgrade.GoldMonitorCount} monitor(s) @ &#163;{_costs.LargeMonitor}";
            upgradeData.AppendChild(gold);
        }
        //         - 5 docks @ &#163;200, 6 monitors @ &#163;0, 4 keyboards @ &#163;25, &#163;3 mice @ &#163;10<br />
        //         Silver Upgrade Costs &#163;2400 - 24 monitors @ &#163;100<br />
        //        <span name="GoldCosts">Gold Upgrade Costs &#163;4650 - 31 monitors @ &#163;150</span></td>

        return upgradeData;
    }
}
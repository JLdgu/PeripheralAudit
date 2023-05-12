using HtmlAgilityPack;
using PeripheralAudit.Application;
using PeripheralAudit.Application.Entities;

namespace PeripheralAudit.Report;

internal sealed class GenerateReport
{
    private PeripheralAuditDbContext _dbContext;
    private string _reportOutput;

    HtmlDocument _template = new HtmlDocument();

    public GenerateReport(PeripheralAuditDbContext dbContext, string reportOutput)
    {
        _dbContext = dbContext;
        _reportOutput = reportOutput;
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
            string htmlOutput = Path.Combine(_reportOutput, HTML_FILE);
            if (!File.Exists(htmlOutput))
                File.Delete(htmlOutput);

            _template.Load(htmlTemplate);

            string reportDate =_template.GetElementbyId("reportDate").InnerHtml;

            HtmlNode reportDateDiv =_template.DocumentNode.SelectSingleNode("//div[@id='reportDate']");
            reportDateDiv.InnerHtml = $"Report produced: {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}";

            HtmlNode reportSite =_template.DocumentNode.SelectSingleNode("//div[@id='reportSite']");
            reportSite.InnerHtml = site.Name;

            HtmlNode reportTable =_template.DocumentNode.SelectSingleNode("//table[@id='reportTable']");

            HtmlNode reportRow =_template.DocumentNode.SelectSingleNode("//tr[@id='reportHeader']");

            List<Location> locations = _dbContext.Locations
                .Where(l => l.Site.Id == site.Id)
                .OrderByDescending(l => l.Name)
                .ToList();

            foreach (Location location in locations)
            {
                var newRow = ReportRow(location);
                reportTable.InsertAfter(newRow, reportRow);
            }

           _template.Save(htmlOutput);

            break;
        }
    }

    private HtmlNode ReportRow(Location location)
    {
        HtmlNode tr = _template.CreateElement("tr");

        HtmlNode name = _template.CreateElement("td");
        name.Attributes.Add("class", "tal");
        name.InnerHtml = location.Name;
        tr.ChildNodes.Append(name);

        HtmlNode desks = TableData(location.DeskCount);
        tr.ChildNodes.Append(desks);

        HtmlNode single = TableData(location.MonitorSingleCount);
        tr.ChildNodes.Append(single);
        HtmlNode dual = TableData(location.MonitorDualCount);
        tr.ChildNodes.Append(dual);
        HtmlNode bronze = TableData(location.MonitorGradeBronzeCount);
        tr.ChildNodes.Append(bronze);
        HtmlNode silver = TableData(location.MonitorGradeSilverCount);
        tr.ChildNodes.Append(silver);
        HtmlNode gold = TableData(location.MonitorGradeGoldCount);
        tr.ChildNodes.Append(gold);
        HtmlNode dock = TableData(location.DockCount);
        tr.ChildNodes.Append(dock);
        // TODO complete table row

        return tr;
    }
    private HtmlNode TableData(int content)
    {
        return TableData(content.ToString());
    }

    private HtmlNode TableData(string content)
    {
        HtmlNode tableData = _template.CreateElement("td");
        tableData.InnerHtml = content;
        return tableData;
    }

    private void TestMethod()
    {
        List<Site> sites = _dbContext.Sites.ToList();
        foreach (Site site in sites)
        {
            Console.WriteLine(site.Name);
            List<Location> locations = _dbContext.Locations
                .Where(l => l.Site.Id == site.Id)
                .OrderByDescending(l => l.Name)
                .ToList();

            foreach (Location location in locations)
            {
                Console.WriteLine(location.Name);
            }
            break;
        }
    }
}
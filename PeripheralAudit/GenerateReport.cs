using HtmlAgilityPack;

internal sealed class GenerateReport
{
    internal void Execute()
    {
        if (!File.Exists("c:/dev/temp/reportTemplate.css"))
            File.Copy("./htmltemplate/reportTemplate.css","c:/dev/temp/reportTemplate.css");
        
        HtmlDocument template = new HtmlDocument();
        template.Load(@".\htmltemplate\reportTemplate.html");

        string reportDate = template.GetElementbyId("reportDate").InnerHtml;

        HtmlNode reportDateDiv = template.DocumentNode.SelectSingleNode("//div[@id='reportDate']");
        reportDateDiv.InnerHtml = $"Report produced: {DateTime.Now.ToLongTimeString()}";

        HtmlNode reportSite = template.DocumentNode.SelectSingleNode("//div[@id='reportSite']");
        reportSite.InnerHtml = "County Hall";

        HtmlNode reportTable = template.DocumentNode.SelectSingleNode("//table[@id='reportTable']");
        
        HtmlNode reportRow = template.DocumentNode.SelectSingleNode("//tr[@id='reportRow']");

        var newNode = reportRow.Clone();
        newNode.Id = "newRow3";
        reportTable.InsertAfter(newNode,reportRow);
        newNode = reportRow.Clone();
        newNode.Id = "newRow4";
        reportTable.InsertAfter(newNode,reportRow);
        newNode = reportRow.Clone();
        newNode.Id = "newRow5";
        reportTable.InsertAfter(newNode,reportRow);

        // foreach (var node in reportTable.ChildNodes)
        // {
        //     if (node.Id == "reportRow")
        //     {   
        //         var newNode = node.Clone();
        //         newNode.Id = "newRow2";
        //         reportTable.InsertAfter(newNode,node);
        //         Console.WriteLine(node.InnerHtml);
        //         break;
        //     }
        // }

        template.Save("c:/dev/temp/auditreport.html");
    }
}
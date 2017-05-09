using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Project_Unite
{
    public static class ChartExtensions
    {
        public static IHtmlString PieChart(this HtmlHelper hpr, string title, Dictionary<string, double> values)
        {
            var sb = new StringBuilder();
            string chart_id = Guid.NewGuid().ToString().Replace("-", "");
            sb.AppendLine($@"<script type=""text/javascript"">
google.charts.load('current', {{'packages':['corechart']}});
      google.charts.setOnLoadCallback(drawChart_{chart_id});

function drawChart_{chart_id}()
{{
  var data_{chart_id} = google.visualization.arrayToDataTable([
          ['Key', 'Value']");

            foreach(var kvs in values)
            {
                sb.Append(",");
                sb.AppendLine($"['{kvs.Key}', {kvs.Value}]");
            }
            sb.AppendLine($@"]);

var options_{chart_id} = {{
          title: '{title}'
        }};

        var chart = new google.visualization.PieChart(document.getElementById('{chart_id}'));

        chart.draw(data, options);");

            sb.AppendLine("}</script>");
            sb.AppendLine($"<div id=\"{chart_id}\" style=\"width:100%; height:auto;\"></div>");
            return hpr.Raw(sb.ToString());
        }
    }
}
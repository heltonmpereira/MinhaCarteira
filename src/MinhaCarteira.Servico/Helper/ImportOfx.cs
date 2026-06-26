using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MinhaCarteira.Servico.Helper;
public static class ImportOfx
{
    public static XElement ToXElement(IEnumerable<string> linhas)
    {
        var tags =
            from line in linhas
            where line.Contains("<STMTTRN>") ||
                  line.Contains("<TRNTYPE>") ||
                  line.Contains("<DTPOSTED>") ||
                  line.Contains("<TRNAMT>") ||
                  line.Contains("<FITID>") ||
                  line.Contains("<CHECKNUM>") ||
                  line.Contains("<MEMO>")
            select line;

        var el = new XElement("root");
        XElement son = null;

        foreach (var l in tags)
        {
            if (l.IndexOf("<STMTTRN>", StringComparison.OrdinalIgnoreCase) != -1)
            {
                son = new XElement("STMTTRN");
                el.Add(son);
                continue;
            }

            var tagName = GetTagName(l);
            var elSon = new XElement(tagName)
            {
                Value = GetTagValue(l, tagName)
            };
            son?.Add(elSon);
        }
        return el;

    }
    /// <summary>
    /// Get the Tag name to create an Xelement
    /// </summary>
    /// <param name="line">One line from the file</param>
    /// <returns></returns>
    private static string GetTagName(string line)
    {
        var posInit = line.IndexOf("<", StringComparison.OrdinalIgnoreCase) + 1;
        var posEnd = line.IndexOf(">", StringComparison.OrdinalIgnoreCase);
        posEnd -= posInit;
        return line.Substring(posInit, posEnd);
    }

    /// <summary>
    /// Get the value of the tag to put on the Xelement
    /// </summary>
    /// <param name="line">The line</param>
    /// <returns></returns>
    private static string GetTagValue(string line, string tagName)
    {
        line = line.Replace($"<{tagName}>", string.Empty);
        line = line.Replace($"&lt;{tagName}&gt;", string.Empty);
        line = line.Replace($"</{tagName}>", string.Empty);
        line = line.Replace($"&lt;/{tagName}&gt;", string.Empty);

        var trimmer = new Regex(@"\s\s+");
        line = trimmer.Replace(line, " ");

        return line.Trim();
    }
}

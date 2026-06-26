using System.Text.RegularExpressions;

namespace MinhaCarteira.AppCliente.Models;

public class BrowserViewModel
{
    public string UserAgent { get; set; }
    public Regex RexSistemaOperacional { get; set; }
    public Regex RexDevice { get; set; }
    public string SistemaOperacional { get; set; }
    public bool IsMobile { get; set; }
}
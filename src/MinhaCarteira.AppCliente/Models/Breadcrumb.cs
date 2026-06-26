namespace MinhaCarteira.AppCliente.Models;

public class Breadcrumb
{
    public string Area { get; set; }
    public string Text { get; set; }
    public string Action { get; set; }
    public string Controller { get; set; }
    public bool Active { get; set; }

    public Breadcrumb() { }

    public Breadcrumb(string text, string action, string controller, string area, bool active)
    {
        this.Text = text;
        this.Action = action;
        this.Controller = controller;
        this.Area = area;
        this.Active = active;
    }
}
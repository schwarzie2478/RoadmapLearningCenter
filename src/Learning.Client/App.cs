using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace Learning.Client;

/// <summary>
/// Root App component implemented in C# (avoids Razor source generator incompatibility).
/// </summary>
public class App : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder __b)
    {
        __b.OpenComponent<Router>(0);
        __b.AddComponentParameter(1, "AppAssembly", typeof(Program).Assembly);
        __b.AddComponentParameter(2, "Found", (RenderFragment<RouteData>)(rd => b =>
        {
            b.OpenComponent<RouteView>(0);
            b.AddComponentParameter(1, "RouteData", rd);
            b.AddComponentParameter(2, "DefaultLayout", typeof(Components.Layout.MainLayout));
            b.CloseComponent();
            b.OpenComponent<FocusOnNavigate>(3);
            b.AddComponentParameter(4, "RouteData", rd);
            b.AddComponentParameter(5, "Selector", "h1");
            b.CloseComponent();
        }));
        __b.AddComponentParameter(3, "NotFound", (RenderFragment)(b =>
        {
            b.OpenElement(0, "div");
            b.OpenElement(1, "h1");
            b.AddContent(2, "404 - Not Found");
            b.CloseElement();
            b.OpenElement(3, "p");
            b.AddContent(4, "Page not found. ");
            b.OpenComponent(5, typeof(NavLink));
            b.AddComponentParameter(6, "href", "/");
            b.AddContent(7, "Return to Home");
            b.CloseComponent();
            b.CloseElement();
            b.CloseElement();
        }));
        __b.CloseComponent();
    }
}

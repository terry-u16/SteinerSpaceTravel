using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SteinerSpaceTravel.Web.Services;

public class CanvasService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ElementReference _canvasElement;

    public CanvasService(IJSRuntime jsRuntime, ElementReference canvasElement)
    {
        _jsRuntime = jsRuntime;
        _canvasElement = canvasElement;
    }

    public async Task DrawPixelsAsync(byte[] pixels)
    {
        await _jsRuntime.InvokeVoidAsync("drawPixels", _canvasElement, pixels);
    }
}
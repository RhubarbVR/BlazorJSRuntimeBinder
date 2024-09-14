using Microsoft.Extensions.DependencyInjection;

namespace BlazorJSRuntimeBinder;

public static class BlazorJSRuntimeBinderHelper
{
	public static void AddBlazorJSRuntimeBinder(this IServiceCollection services) {
		services.AddScoped((builder) => new BlazorJSBinderContext(builder.GetService<IJSRuntime>()));
	}

}
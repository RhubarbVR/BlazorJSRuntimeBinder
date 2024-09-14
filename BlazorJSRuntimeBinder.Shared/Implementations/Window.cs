using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Text.Json;

using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;
using Microsoft.JSInterop.Infrastructure;

namespace BlazorJSRuntimeBinder;


public class Window(BlazorJSBinderContext blazorJSBinderContext, IJSObjectReference jSObjectReference) : JSObject(blazorJSBinderContext, jSObjectReference), IJSGlobalPropertyObjectConstructor
{
	public static string GlobalProperty => "window";

	public static new IJSObject CreateJSObjectLink(BlazorJSBinderContext blazorJSBinderContext, IJSObjectReference jSObjectReference) {
		return new Window(blazorJSBinderContext, jSObjectReference);
	}

	public ValueTask<string> name => Get_name();

	public ValueTask<string> Get_name() {
		return Context.ReflectGet<string>(ObjectReference, "name");
	}

	public ValueTask Set_name(string name) {
		return Context.ReflectSet(ObjectReference, "name", name);
	}

	public ValueTask alert(string alert) {
		return Context.InvokeVoidAsync(ObjectReference, "alert", [alert]);
	}
}
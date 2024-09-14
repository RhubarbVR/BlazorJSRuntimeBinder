using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Text.Json;

using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;
using Microsoft.JSInterop.Infrastructure;

namespace BlazorJSRuntimeBinder;

public class JSObject(BlazorJSBinderContext blazorJSBinderContext, IJSObjectReference jSObjectReference) : IJSObjectConstructor
{
	public readonly IJSObjectReference ObjectReference = jSObjectReference;
	public readonly BlazorJSBinderContext Context = blazorJSBinderContext;

	IJSObjectReference IJSObject.ObjectReference => ObjectReference;

	public static IJSObject CreateJSObjectLink(BlazorJSBinderContext blazorJSBinderContext, IJSObjectReference jSObjectReference) {
		return new JSObject(blazorJSBinderContext, jSObjectReference);
	}

	public ValueTask<string> TypeOf() {
		return Context.TypeOf(ObjectReference);
	}

	public ValueTask<string> FullTypeOf() {
		return Context.FullTypeOf(ObjectReference);
	}

	public ValueTask DisposeAsync() {
		GC.SuppressFinalize(this);
		return ObjectReference.DisposeAsync();
	}

	~JSObject() {
		Task.Run(DisposeAsync);
	}

	public override string ToString() {
		return $"{{CSharpType: {GetType()}, ObjectReference:{ObjectReference}}}";
	}
}
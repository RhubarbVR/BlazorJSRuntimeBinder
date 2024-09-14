using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;

namespace BlazorJSRuntimeBinder;

public interface IJSObject : IAsyncDisposable
{
	public IJSObjectReference ObjectReference { get; }
}
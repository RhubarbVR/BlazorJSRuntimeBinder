namespace BlazorJSRuntimeBinder;

public interface IJSGlobalPropertyObjectConstructor : IJSObjectConstructor
{
	public static abstract string GlobalProperty { get; }
}
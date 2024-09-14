namespace BlazorJSRuntimeBinder;

public interface IJSObjectConstructor : IJSObject
{
	public static abstract IJSObject CreateJSObjectLink(BlazorJSBinderContext blazorJSBinderContext, IJSObjectReference jSObjectReference);
}
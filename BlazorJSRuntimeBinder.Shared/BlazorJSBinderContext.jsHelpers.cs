namespace BlazorJSRuntimeBinder;

public sealed partial class BlazorJSBinderContext
{
	private async Task LoadInJSHelperFunctions() {
		await AddNewGlobalHelperFunction(nameof(GetGlobal), "()=>globalThis;");
		await AddNewGlobalHelperFunction(nameof(NewRef), "(arg)=>arg;");
		await AddNewGlobalHelperFunction(nameof(TypeOf), "(arg)=>typeof arg;");
		await AddNewGlobalHelperFunction(nameof(FullTypeOf),
@"(arg)=>{
	let type = typeof arg;
	if(type ==='object') {
		try {
			return arg.constructor.name;
		} catch {
		}
	}
	return type;
}");

		Global = await GetGlobal();
	}

	public IJSObjectReference Global { get; private set; }

	public ValueTask<string> FullTypeOf(IJSObjectReference jSObjectReference) {
		return InvokeHelperAsync<string>(nameof(FullTypeOf), jSObjectReference);
	}

	public ValueTask<string> TypeOf(IJSObjectReference jSObjectReference) {
		return InvokeHelperAsync<string>(nameof(TypeOf), jSObjectReference);
	}

	public ValueTask<IJSObjectReference> NewRef(IJSObjectReference jSObjectReference) {
		return InvokeHelperAsync<IJSObjectReference>(nameof(NewRef), jSObjectReference);
	}

	public ValueTask<IJSObjectReference> GetGlobal() {
		return InvokeHelperAsync<IJSObjectReference>(nameof(GetGlobal));
	}
}

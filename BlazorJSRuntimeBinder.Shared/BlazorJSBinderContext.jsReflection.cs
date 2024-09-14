namespace BlazorJSRuntimeBinder;

public sealed partial class BlazorJSBinderContext
{
	/// <summary>
	/// Get Property
	/// </summary>
	/// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
	/// <returns>An instance of <typeparamref name="TValue"/> obtained by JSON-DeSerializing the return value.</returns>
	public async ValueTask<TValue> ReflectGet<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(IJSObjectReference target, string propName) {
		await Load();
		return await InvokeAsync<TValue>("Reflect.get", [target, propName]);
	}

	/// <summary>
	/// Set Property
	/// </summary>
	public async ValueTask ReflectSet(IJSObjectReference target, string propName, object value) {
		await Load();
		await InvokeVoidAsync("Reflect.set", [target, propName, value]);
	}

	/// <summary>
	/// Set Property
	/// </summary>
	public async ValueTask ReflectSet(string propName, object value) {
		await Load();
		await ReflectSet(Global, propName, value);
	}

	/// <summary>
	/// Get Global Property
	/// </summary>
	/// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
	/// <returns>An instance of <typeparamref name="TValue"/> obtained by JSON-DeSerializing the return value.</returns>
	public async ValueTask<TValue> ReflectGet<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string propName) {
		await Load();
		return await ReflectGet<TValue>(Global, propName);
	}

	/// <summary>
	/// Get Global Property
	/// </summary>
	public async ValueTask<IJSObjectReference> GetGlobal(string proName) {
		await Load();
		return await ReflectGet<IJSObjectReference>(Global, proName);
	}
}

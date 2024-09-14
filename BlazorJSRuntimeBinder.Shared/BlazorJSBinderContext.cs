using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BlazorJSRuntimeBinder;

public sealed partial class BlazorJSBinderContext
{
	public BlazorJSBinderContext(IJSRuntime jSRuntime) {
		ArgumentNullException.ThrowIfNull(jSRuntime);
		JsRuntime = jSRuntime;
	}

	public bool IsLoaded { get; private set; }

	public async Task Load() {
		if (IsLoaded) {
			return;
		}
		IsLoaded = true;
		await LoadInJSHelperFunctions();
	}

	public readonly IJSRuntime JsRuntime;

	public T CreateBinding<T>(IJSObjectReference reference) where T : class, IJSObjectConstructor {
		return (T)T.CreateJSObjectLink(this, reference);
	}


	public async ValueTask<T> GetGlobal<T>() where T : class, IJSGlobalPropertyObjectConstructor {
		return CreateBinding<T>(await GetGlobal(T.GlobalProperty));
	}

	/// <summary>
	/// Flags for a member that is JSON (de)serialized.
	/// </summary>
	private const DynamicallyAccessedMemberTypes JSON_SERIALIZED = DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties;

	public ValueTask RunJSEval(string code) {
		return JsRuntime.InvokeVoidAsync("eval", code);
	}

	public ValueTask AddNewGlobalFunction(string functionName, string lambdaFunction) {
		return RunJSEval($"globalThis.{functionName} = {lambdaFunction}");
	}

	public const string HELPER_FUNCTIONS_LABEL = "BlazorJS_Helper_";

	public ValueTask AddNewGlobalHelperFunction(string functionName, string lambdaFunction) {
		return RunJSEval($"globalThis.{HELPER_FUNCTIONS_LABEL}{functionName} = {lambdaFunction}");
	}


	/// <summary>
	/// Invokes the specified JavaScript function asynchronously.
	/// </summary>
	/// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
	/// <param name="args">JSON-serializable arguments.</param>
	/// <returns>A <see cref="ValueTask"/> that represents the asynchronous invocation operation.</returns>
	public ValueTask InvokeHelperVoidAsync(string identifier, params object[] args) {
		return InvokeVoidAsync($"{HELPER_FUNCTIONS_LABEL}{identifier}", args);
	}

	/// <summary>
	/// Invokes the specified JavaScript function asynchronously.
	/// <para>
	/// <see cref="JSRuntime"/> will apply timeouts to this operation based on the value configured in <see cref="JSRuntime.DefaultAsyncTimeout"/>. To dispatch a call with a different timeout, or no timeout,
	/// consider using <see cref="IJSRuntime.InvokeAsync{TValue}(string, CancellationToken, object[])" />.
	/// </para>
	/// </summary>
	/// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
	/// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
	/// <param name="args">JSON-serializable arguments.</param>
	/// <returns>An instance of <typeparamref name="TValue"/> obtained by JSON-(de)serializing the return value.</returns>
	public ValueTask<TValue> InvokeHelperAsync<[DynamicallyAccessedMembers(JSON_SERIALIZED)] TValue>(string identifier, params object[] args) {
		return InvokeAsync<TValue>($"{HELPER_FUNCTIONS_LABEL}{identifier}", args);
	}


}

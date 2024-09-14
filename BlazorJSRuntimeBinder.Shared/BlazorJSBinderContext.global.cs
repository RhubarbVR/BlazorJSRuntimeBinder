namespace BlazorJSRuntimeBinder;

public sealed partial class BlazorJSBinderContext
{

	/// <summary>
	/// Invokes the specified JavaScript function asynchronously.
	/// <para>
	/// <see cref="JSRuntime"/> will apply timeouts to this operation based on the value configured in <see cref="JSRuntime.DefaultAsyncTimeout"/>. To dispatch a call with a different timeout, or no timeout,
	/// consider using <see cref="InvokeAsync{TValue}(string, CancellationToken, object[])" />.
	/// </para>
	/// </summary>
	/// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
	/// <param name="args">JSON-serializable arguments.</param>
	/// <returns>An instance of <typeparamref name="TValue"/> obtained by JSON-DeSerializing the return value.</returns>
	public async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object[] args) {
		await Load();

		if (JsRuntime is IJSInProcessRuntime inProcessRuntime) {
			return inProcessRuntime.Invoke<TValue>(identifier, args);
		}

		return await JsRuntime.InvokeAsync<TValue>(identifier, args);
	}

	/// <summary>
	/// Invokes the specified JavaScript function asynchronously.
	/// </summary>
	/// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
	/// <param name="cancellationToken">
	/// A cancellation token to signal the cancellation of the operation. Specifying this parameter will override any default cancellations such as due to timeouts
	/// (<see cref="JSRuntime.DefaultAsyncTimeout"/>) from being applied.
	/// </param>
	/// <param name="args">JSON-serializable arguments.</param>
	/// <returns>An instance of <typeparamref name="TValue"/> obtained by JSON-(de)serialized the return value.</returns>
	public async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object[] args, CancellationToken cancellationToken) {
		await Load();
		
		if (JsRuntime is IJSInProcessRuntime inProcessRuntime) {
			return inProcessRuntime.Invoke<TValue>(identifier, args);
		}

		return await JsRuntime.InvokeAsync<TValue>(identifier, cancellationToken, args);
	}

	/// <summary>
	/// Invokes the specified JavaScript function asynchronously.
	/// </summary>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
	/// <param name="args">JSON-serializable arguments.</param>
	/// <returns>A <see cref="ValueTask"/> that represents the asynchronous invocation operation.</returns>
	public async ValueTask InvokeVoidAsync(string identifier, object[] args) {
		await Load();
		
		if (JsRuntime is IJSInProcessRuntime inProcessRuntime) {
			inProcessRuntime.InvokeVoid(identifier, args);
			return;
		}

		await JsRuntime.InvokeAsync<IJSVoidResult>(identifier, args);
	}

	/// <summary>
	/// Invokes the specified JavaScript function asynchronously.
	/// </summary>
	/// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
	/// <param name="cancellationToken">
	/// A cancellation token to signal the cancellation of the operation. Specifying this parameter will override any default cancellations such as due to timeouts
	/// (<see cref="JSRuntime.DefaultAsyncTimeout"/>) from being applied.
	/// </param>
	/// <param name="args">JSON-serializable arguments.</param>
	/// <returns>An instance of <typeparamref name="TValue"/> obtained by JSON-(de)serialized the return value.</returns>
	public async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(JSON_SERIALIZED)] TValue>(string identifier, CancellationToken cancellationToken,  object[] args) {
		await Load();
		
		if (JsRuntime is IJSInProcessRuntime inProcessRuntime) {
			return inProcessRuntime.Invoke<TValue>(identifier, args);
		}
		return await JsRuntime.InvokeAsync<TValue>(identifier, cancellationToken, args);
	}

	/// <summary>
	/// Invokes the specified JavaScript function asynchronously.
	/// </summary>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
	/// <param name="cancellationToken">
	/// A cancellation token to signal the cancellation of the operation. Specifying this parameter will override any default cancellations such as due to timeouts
	/// (<see cref="JSRuntime.DefaultAsyncTimeout"/>) from being applied.
	/// </param>
	/// <param name="args">JSON-serializable arguments.</param>
	/// <returns>A <see cref="ValueTask"/> that represents the asynchronous invocation operation.</returns>
	public async ValueTask InvokeVoidAsync(string identifier, CancellationToken cancellationToken, object[] args) {
		await Load();
		
		if (JsRuntime is IJSInProcessRuntime inProcessRuntime) {
			inProcessRuntime.InvokeVoid(identifier, args);
			return;
		}
		await JsRuntime.InvokeAsync<IJSVoidResult>(identifier, cancellationToken, args);
	}

	/// <summary>
	/// Invokes the specified JavaScript function asynchronously.
	/// </summary>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
	/// <param name="timeout">The duration after which to cancel the async operation. Overrides default timeouts (<see cref="JSRuntime.DefaultAsyncTimeout"/>).</param>
	/// <param name="args">JSON-serializable arguments.</param>
	/// <returns>A <see cref="ValueTask"/> that represents the asynchronous invocation operation.</returns>
	public async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(JSON_SERIALIZED)] TValue>(string identifier, TimeSpan timeout,  object[] args) {
		await Load();
		
		if (JsRuntime is IJSInProcessRuntime inProcessRuntime) {
			return inProcessRuntime.Invoke<TValue>(identifier, args);
		}

		using var cancellationTokenSource = timeout == Timeout.InfiniteTimeSpan ? null : new CancellationTokenSource(timeout);
		var cancellationToken = cancellationTokenSource?.Token ?? CancellationToken.None;

		return await JsRuntime.InvokeAsync<TValue>(identifier, cancellationToken, args);
	}

	/// <summary>
	/// Invokes the specified JavaScript function asynchronously.
	/// </summary>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
	/// <param name="timeout">The duration after which to cancel the async operation. Overrides default timeouts (<see cref="JSRuntime.DefaultAsyncTimeout"/>).</param>
	/// <param name="args">JSON-serializable arguments.</param>
	/// <returns>A <see cref="ValueTask"/> that represents the asynchronous invocation operation.</returns>
	public async ValueTask InvokeVoidAsync(string identifier, TimeSpan timeout, object[] args) {
		await Load();
		
		if (JsRuntime is IJSInProcessRuntime inProcessRuntime) {
			inProcessRuntime.InvokeVoid(identifier, args);
			return ;
		}

		using var cancellationTokenSource = timeout == Timeout.InfiniteTimeSpan ? null : new CancellationTokenSource(timeout);
		var cancellationToken = cancellationTokenSource?.Token ?? CancellationToken.None;

		await JsRuntime.InvokeAsync<IJSVoidResult>(identifier, cancellationToken, args);
	}
}

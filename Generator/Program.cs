using System.Diagnostics;

namespace Generator;

internal class Program
{
	public static MDNReader MDNData;

	static string GetPath(string path, int depth = 0) {
		path = Path.GetFullPath(path);
		if (File.Exists(Path.Combine(path, "BlazorJSRuntimeBinder.sln"))) {
			return path;
		}
		if (depth >= 100) {
			throw new Exception("Failed to find path");
		}
		return GetPath(Path.Combine(path, ".."), depth + 1);
	}

	public static string TargetPath;

	private static async Task Start() {
		Console.WriteLine("RunningMDN reader Path:" + TargetPath);
		MDNData = new MDNReader();
		await MDNData.StartReader(TargetPath);
	}

	private static async Task GenerateFiles() {

	}

	static async Task Main(string[] args) {
		try {
			string path = null;
			if (args.Length == 1) {
				try {
					path = GetPath(args[0]);
				}
				catch { }
			}
			path ??= GetPath("./");
			TargetPath = path;
			var timer = Stopwatch.StartNew();
			await Start();
			timer.Stop();
			Console.WriteLine($"{timer.Elapsed} to start");
			timer.Restart();
			await GenerateFiles();
			timer.Stop();
			Console.WriteLine($"{timer.Elapsed} to generate code");
			Console.ReadLine();
		}
		catch (Exception error) {
			Console.WriteLine("Error " + error);
			Console.ReadLine();
			throw;
		}
	}
}

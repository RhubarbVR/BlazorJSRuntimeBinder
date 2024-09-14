using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.AccessControl;

namespace Generator;

[Flags]
public enum MDNFlags
{
	None = 0,
	Deprecated = 1 << 0,
	Experimental = 1 << 1,
	ReadOnly = 1 << 2,
	SecureContext = 1 << 3,
	NonStandard = 1 << 4,
}

public struct PageInfo
{
	public string Name;
	public string PageType;
	public string Path;
	public string BrowserCompat;
	public string Slug;

	public MDNFlags Flags;
}

public sealed class MDNReader
{
	public string TargetPath { get; private set; }

	public string TranslatedContent => Path.Combine(TargetPath, "translated-content");
	public string Content => Path.Combine(TargetPath, "content");
	public string BrowserCompatData => Path.Combine(TargetPath, "browser-compat-data.json");

	public string InterfaceData => Path.Combine(TargetPath, "content", "files", "jsondata", "InterfaceData.json");

	public string WebApi => Path.Combine(TargetPath, "content", "files", "en-us", "web", "api");
	public string JavaScriptGlobalObjects => Path.Combine(TargetPath, "content", "files", "en-us", "web", "javascript", "reference", "global_objects");

	public JObject BrowserCompat { get; private set; }
	public JObject Interfaces { get; private set; }

	public Dictionary<string, API> APIS { get; private set; }

	public static async Task<List<PageInfo>> GetPages(params string[] targetDirs) {
		var infos = new List<PageInfo>();
		foreach (var dir in targetDirs) {
			foreach (var path in Directory.GetDirectories(dir)) {
				var targetFile = Path.Combine(path, "index.md");
				if (!File.Exists(targetFile)) {
					continue;
				}
				string title = null;
				string slug = null;
				string pageType = null;
				string browserCompat = null;
				var pageFlags = MDNFlags.None;
				{
					using var read = File.OpenRead(targetFile);
					using var stringReader = new StreamReader(read);
					for (var i = 0; i < 9; i++) {
						var line = await stringReader.ReadLineAsync();
						if (line.StartsWith("title:")) {
							title = line.Replace("title: ", null).Replace("title:", null);
						}
						if (line.StartsWith("page-type:")) {
							pageType = line.Replace("page-type: ", null).Replace("page-type:", null);
						}
						if (line.StartsWith("browser-compat:")) {
							browserCompat = line.Replace("browser-compat: ", null).Replace("browser-compat:", null);
						}
						if (line.StartsWith("slug:")) {
							slug = line.Replace("slug: ", null).Replace("slug:", null);
						}
						if (line.Contains("experimental") && line.Contains('-')) {
							pageFlags |= MDNFlags.Experimental;
						}
						if (line.Contains("deprecated") && line.Contains('-')) {
							pageFlags |= MDNFlags.Deprecated;
						}
					}
				}
				if (title is null || pageType is null) {
					continue;
				}
				infos.Add(new PageInfo { Name = title, Path = path, PageType = pageType, BrowserCompat = browserCompat });
			}
		}
		return infos;
	}


	public PageInfo[] AllPages { get; private set; }

	public Dictionary<string, PageInfo[]> TypePageInfo { get; private set; }

	public async Task StartReader(string targetPath) {
		TargetPath = Path.GetFullPath(targetPath);
		CheckForNeededFiles();

		{
			using var file = File.OpenText(BrowserCompatData);
			using var reader = new JsonTextReader(file);
			BrowserCompat = (JObject)await JToken.ReadFromAsync(reader);
		}

		{
			using var file = File.OpenText(InterfaceData);
			using var reader = new JsonTextReader(file);
			Interfaces = (JObject)((JArray)await JToken.ReadFromAsync(reader))[0];
		}

		{
			AllPages = [.. (await GetPages(WebApi, JavaScriptGlobalObjects)), .. (await GetPages(Directory.GetDirectories(WebApi)))];
			TypePageInfo = AllPages.GroupBy(x => x.PageType).ToDictionary(x => x.Key, x => x.ToArray());
			foreach (var item in TypePageInfo) {
				Console.WriteLine($"{item.Key} has {item.Value.Length} values");
			}
		}
		await LoadTypes();
		PopulateBaseTypes();

		Console.WriteLine($"There are {TypeLookUp.Count} Types being made and {AllPages.Length} pages");
	}

	public void PopulateBaseTypes() {
		JS_Symbol = TypeLookUp.GetValueOrDefault("Symbol") ?? throw new NotImplementedException();
		JS_String = TypeLookUp.GetValueOrDefault("String") ?? throw new NotImplementedException();
		JS_Number = TypeLookUp.GetValueOrDefault("Number") ?? throw new NotImplementedException();
		JS_Boolean = TypeLookUp.GetValueOrDefault("Boolean") ?? throw new NotImplementedException();
		JS_Function = TypeLookUp.GetValueOrDefault("Function") ?? throw new NotImplementedException();
		JS_Array = TypeLookUp.GetValueOrDefault("Array") ?? throw new NotImplementedException();
	}

	public JSType JS_Symbol { get; private set; }
	public JSType JS_String { get; private set; }
	public JSType JS_Number { get; private set; }
	public JSType JS_Boolean { get; private set; }
	public JSType JS_Function { get; private set; }
	public JSType JS_Array { get; private set; }

	public IEnumerable<JSType> AllTypes => TypeLookUp.Values;

	public readonly Dictionary<string, JSType> TypeLookUp = [];

	private async Task<JSType> InitType(PageInfo pageInfo) {
		var type = new JSType();
		TypeLookUp.Add(pageInfo.Name, type);
		await type.LoadType(LoadStep.Start, this, pageInfo);
		return type;
	}

	private async Task ExtraTypeLoad(LoadStep loadStep) {
		foreach (var type in AllTypes) {
			await type.LoadType(loadStep, this, type.PageInfo);
		}
	}

	private async Task LoadTypes() {
		foreach (var pageInfo in TypePageInfo["web-api-interface"]) {
			await InitType(pageInfo);
		}
		foreach (var pageInfo in TypePageInfo["javascript-class"]) {
			await InitType(pageInfo);
		}
		await ExtraTypeLoad(LoadStep.Mid);
		await ExtraTypeLoad(LoadStep.SubData);
		await ExtraTypeLoad(LoadStep.Reliant);
		await ExtraTypeLoad(LoadStep.ReliantTree);
		await ExtraTypeLoad(LoadStep.End);
	}

	private static void NeedToPullSubmodules() {
		throw new Exception("Need to Pull Submodules");
	}

	private void CheckForNeededFiles() {
		if (!Directory.Exists(TargetPath)) {
			throw new Exception("MDN path invalid");
		}
		if (!File.Exists(Path.Combine(TranslatedContent, "README.md"))) {
			NeedToPullSubmodules();
		}
		if (!File.Exists(Path.Combine(Content, "README.md"))) {
			NeedToPullSubmodules();
		}
		if (!File.Exists(BrowserCompatData)) {
			throw new Exception("Need to have browser-compat-data downloaded");
		}
	}
}

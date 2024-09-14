using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Generator;

public enum JSTypeType
{
	Undefined,
	Interface,
	JSClass,
}

public enum SubDataType
{
	None,
	Properties,
	Methods,
	Events,
}

public sealed class JSType
{
	public string Name => PageInfo.Name;
	public JSTypeType Type { get; private set; }

	public MDNFlags Flags => PageInfo.Flags;

	public readonly List<JSType> ParentTypes = [];
	public readonly List<JSType> ChildTypes = [];

	public readonly HashSet<JSType> ReliantTypes = [];

	public PageInfo PageInfo { get; private set; }

	public struct SubData
	{
		public SubDataType Type;
		public MDNFlags Flags;
		public string FullCodeLine;
	}

	private static string CleanUpString(string s) {
		if (!s.Contains('"')) {
			return s;
		}
		var index = s.IndexOf('\"');
		s = s.Substring(index + 1);
		index = s.IndexOf('\"');
		return s.Remove(index);
	}

	private static string[] GetTargets(string codeLine) {
		var splits = codeLine.Split(",");
		var targets = new string[splits.Length];
		for (var i = 0; i < targets.Length; i++) {
			targets[i] = CleanUpString(splits[i]);
		}
		return targets;
	}

	public readonly List<JSProperty> Properties = [];
	public readonly List<JSMethod> Methods = [];
	public readonly List<JSEvent> Events = [];
	public readonly List<JSSubData> SubInfo = [];

	private async Task GenerateSubData(MDNReader mdnReader, SubData subData) {
		var targets = GetTargets(subData.FullCodeLine);
		switch (subData.Type) {
			case SubDataType.Properties:
				var prop = new JSProperty();
				Properties.Add(prop);
				SubInfo.Add(prop);
				await prop.Load(mdnReader, subData, targets);
				break;
			case SubDataType.Methods:
				var jsMethod = new JSMethod();
				Methods.Add(jsMethod);
				SubInfo.Add(jsMethod);
				await jsMethod.Load(mdnReader, subData, targets);
				break;
			case SubDataType.Events:
				var jSEvent = new JSEvent();
				Events.Add(jSEvent);
				SubInfo.Add(jSEvent);
				await jSEvent.Load(mdnReader, subData, targets);
				break;
			default:
				break;
		}
	}

	private async Task LoadSubData(MDNReader mdnReader) {
		using var reader = File.OpenRead(Path.Combine(PageInfo.Path, "index.md"));
		{
			using var streamReader = new StreamReader(reader);
			var currentType = SubDataType.None;
			List<SubData> SubDatas = [];
			while (!streamReader.EndOfStream) {
				var line = await streamReader.ReadLineAsync();
				var lowerLine = line.ToLower();
				if (line.StartsWith("##")) {
					if (lowerLine.Contains("properties")) {
						currentType = SubDataType.Properties;
					}
					else if (lowerLine.Contains("methods")) {
						currentType = SubDataType.Methods;
					}
					else if (lowerLine.Contains("events")) {
						currentType = SubDataType.Events;
					}
				}
				if (currentType is SubDataType.None) {
					continue;
				}
				if (!line.StartsWith("- {{")) {
					continue;
				}
				var endPoint = line.IndexOf("}}");
				if (endPoint == -1) {
					continue;
				}
				var subDataFlags = MDNFlags.None;
				if (lowerLine.Contains("deprecated")) {
					subDataFlags |= MDNFlags.Deprecated;
				}
				if (lowerLine.Contains("readonly") || lowerLine.Contains("read-only")) {
					subDataFlags |= MDNFlags.ReadOnly;
				}
				if (lowerLine.Contains("experimental")) {
					subDataFlags |= MDNFlags.Experimental;
				}
				if (lowerLine.Contains("securecontext") || lowerLine.Contains("secure-context")) {
					subDataFlags |= MDNFlags.SecureContext;
				}
				if (lowerLine.Contains("non-standard") || lowerLine.Contains("nonstandard")) {
					subDataFlags |= MDNFlags.NonStandard;
				}
				var codeLine = line.Substring(4, endPoint - 4);
				SubDatas.Add(new SubData { Flags = subDataFlags, FullCodeLine = codeLine, Type = currentType });
			}
			foreach (var subData in SubDatas) {
				await GenerateSubData(mdnReader, subData);
			}
		}
	}

	private void AddReliantType(JSType jSType) {
		ReliantTypes.Add(jSType);
		foreach (var item in ChildTypes) {
			item.AddReliantType(jSType);
		}
	}

	private void LoadReliantTree() {
		foreach (var type in ReliantTypes) {
			AddReliantType(type);
		}
	}

	public async Task LoadType(LoadStep loadStep, MDNReader mdnReader, PageInfo pageInfo) {
		if (loadStep is LoadStep.Start) {
			PageInfo = pageInfo;
			Type = pageInfo.PageType switch {
				"javascript-class" => JSTypeType.JSClass,
				"web-api-interface" => JSTypeType.Interface,
				_ => throw new NotImplementedException($"Not supported Type {pageInfo.PageType}"),
			};
		}
		if (loadStep is LoadStep.Mid) {
			var targetObject = mdnReader.Interfaces[Name];
			if (targetObject is not null) {
				string[] inherent = [(string)targetObject["inh"], .. ((JArray)targetObject["impl"]).Select(x => (string)x)];
				foreach (var item in inherent) {
					if (mdnReader.TypeLookUp.TryGetValue(item, out var parentType)) {
						ParentTypes.Add(parentType);
						parentType.ChildTypes.Add(this);
					}
				}
			}
		}
		if (loadStep is LoadStep.SubData) {
			await LoadSubData(mdnReader);
		}
		if (loadStep is LoadStep.Reliant) {
			foreach (var type in ParentTypes) {
				AddReliantType(type);
			}
		}
		if (loadStep is LoadStep.ReliantTree) {
			LoadReliantTree();
		}
	}
}

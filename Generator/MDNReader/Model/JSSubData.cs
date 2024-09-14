using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Generator;

public abstract class JSSubData
{
	public string Name => PageInfo.Name;

	public MDNFlags Flags => PageInfo.Flags | SubData.Flags;

	public JSType.SubData SubData { get; private set; }

	public SubDataType SubDataType => SubData.Type;

	public PageInfo PageInfo { get; protected set; }

	public async Task Load(MDNReader mdnReader, JSType.SubData subData, string[] targets) {
		SubData = subData;
		await LoadData(mdnReader, subData, targets);
	}

	public abstract Task LoadData(MDNReader mdnReader, JSType.SubData subData, string[] targets);
}

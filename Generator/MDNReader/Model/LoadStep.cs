using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Generator;

public enum LoadStep
{
	Start,
	Mid,
	SubData,
	Reliant,
	ReliantTree,
	End,
}

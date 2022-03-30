using Newtonsoft.Json.Linq;

namespace Hilo.ActionJSON
{
	public class ActionJSON
	{
		public string type;
		public Payload payload;

		public ActionJSON(string type, Payload payload)
		{
			this.type = type;
			this.payload = payload;
		}

		public static string GetJSONFromPayload<T>(string type, T payload) where T : Payload
		{
			JObject json = new JObject();

			json["type"] = type;
			json["payload"] = JToken.FromObject(payload);

			return (json.ToString());
		}
	}
}
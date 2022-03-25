using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hilo.ActionJSON
{
	public class ActionHandler
	{
		private Dictionary<string, Type> typesFromString = new Dictionary<string, Type>();
		private Dictionary<string, object> handlerFromString = new Dictionary<string, object>();

		public void JSONMessageHandler(string json)
		{
			ActionJSON action = JsonUtility.FromJson<ActionJSON>(json);

			if (action == null)
			{
				Debug.Log("Unkown action");
				return;
			}

			JToken payloadJObj =  JObject.Parse(json).GetValue("payload");
			object parsed = payloadJObj.ToObject(typesFromString[action.type]);
			action.payload = (Payload)parsed;

			DoAction(action);
		}

		public void AddActionType<T>(string actionType, Action<T> handler) where T : Payload
		{
			typesFromString.Add(actionType, typeof(T));
			handlerFromString.Add(actionType, handler);
		}

		public void DoAction(Hilo.ActionJSON.ActionJSON action)
		{
			if (!typesFromString.ContainsKey(action.type) || !handlerFromString.ContainsKey(action.type))
				return;

			typeof(ActionHandler)
				.GetMethod("CallHandler", BindingFlags.NonPublic | BindingFlags.Instance)
				.MakeGenericMethod((typesFromString[action.type]))
				.Invoke(this, new object[] {
					action.payload,
					handlerFromString[action.type]
				});
		}

		private void CallHandler<T>(T payload, object handler)
		{
			Action<T> typedhandler = (Action<T>)handler;

			typedhandler.Invoke(payload);
		}
	}
}
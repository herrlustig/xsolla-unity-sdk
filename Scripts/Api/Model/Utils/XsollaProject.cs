using SimpleJSON;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Xsolla 
{
	public class XsollaProject : IParseble
	{

		public long 	id					{get; private set;}// "id":14244,
		public long 	merchantId			{get; private set;}// "merchantId":2340,
		
		public int 		recurringPackageCount{get; private set;}// "recurringPackageCount":0,

		public string 	name				{get; private set;}// "name":"TestCreateProject",
		public string 	nameEn				{get; private set;}// "nameEn":"TestCreateProject",
		public string 	virtualCurrencyName	{get; private set;}// "virtualCurrencyName":"virt. currency",
		public string 	virtualCurrencyIconUrl	{get; private set;}// virtualCurrencyImage:"//cdn3.xsolla.com/img/misc/images/91d3aecf770347428c8c6abdc8a260b8.png"
		public string 	projectUrl			{get; private set;}// "projectUrl":"xsolla.com",
		public string 	returnUrl			{get; private set;}// "returnUrl":"https:\/\/secure.xsolla.com?v1=user_1&v2=John+Smith",
		public string 	eula				{get; private set;}// "eula":"http:\/\/xsolla.com\/termsandconditions\/?lang=en&ca=2340",

		public bool 	isDiscrete			{get; private set;}// "isDiscrete":false,
		public bool 	isKeepUsers			{get; private set;}// "isKeepUsers":false,
		public bool 	canRepeatPayment	{get; private set;}// "canRepeatPayment":true

		public Dictionary<string, XComponent> components {get; private set;}

		public XsollaProject() {
			components = new Dictionary<string, XComponent> ();
		}

		public IParseble Parse (JSONNode projectNode)
		{
			id = projectNode ["id"].AsInt;
			name = projectNode ["name"];
			nameEn = projectNode ["nameEn"];
			virtualCurrencyName = projectNode ["virtualCurrencyName"];
			virtualCurrencyIconUrl = projectNode ["virtualCurrencyImage"];
			virtualCurrencyIconUrl = virtualCurrencyIconUrl.Replace("//","");
			merchantId = projectNode ["merchantId"].AsInt;
			isDiscrete = projectNode ["isDiscrete"].AsBool;
			projectUrl = projectNode ["projectUrl"];
			returnUrl = projectNode ["returnUrl"];
			isKeepUsers = projectNode ["isKeepUsers"].AsBool;
			recurringPackageCount = projectNode ["recurringPackageCount"].AsInt;
			eula = projectNode ["eula"];
			canRepeatPayment = projectNode ["canRepeatPayment"].AsBool;
			
			JSONClass jsonObj = projectNode["components"].AsObject;

			if (jsonObj == null)
				return this;

			IEnumerator elements = jsonObj.GetEnumerator();
			while (elements.MoveNext()) {
				KeyValuePair<string, JSONNode> elem = (KeyValuePair<string, JSONNode>)elements.Current;
				string localName = elem.Value["name"].Value;
				bool isEnabled = elem.Value["enabled"].AsBool;
				Debug.Log ("elem.Key " + elem.Key + " name " + localName + " isEnabled " + isEnabled);
				XComponent newComponent = new XComponent(localName, isEnabled);
				components.Add(elem.Key, newComponent);
			}
			return this;
		}

		public override string ToString ()
		{
			return string.Format ("[XsollaProject: id={0}, name={1}, nameEn={2}, virtualCurrencyName={3}, merchantId={4}, isDiscrete={5}, projectUrl={6}, returnUrl={7}, isKeepUsers={8}, recurringPackageCount={9}, eula={10}, canRepeatPayment={11}]", id, name, nameEn, virtualCurrencyName, merchantId, isDiscrete, projectUrl, returnUrl, isKeepUsers, recurringPackageCount, eula, canRepeatPayment);
		}
	}

	public struct XComponent {
		public string 	Name;
		public bool	IsEnabled;

		public XComponent(string newName, bool isEnabled) {
			Name 		= newName == "null" ? "" : newName ;
			IsEnabled 	= isEnabled;
		}
	}
}

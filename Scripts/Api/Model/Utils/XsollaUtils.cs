using System.Collections;
using SimpleJSON;

namespace Xsolla 
{
	public class XsollaUtils : IParseble
	{
		private string 				accessToken;//"access_token":"smW5Oz1jlWv8JRelOXVnshdmQm2BdnA9"
		private XsollaUser 			user;// "user":{},
		private XsollaProject 		project;// "project":{},
		private XsollaPurchase 		purchase;// "purchase":[],
		private XsollaSettings 		settings;// "settings":{},
		private XsollaTranslations 	translations;// "translations":{},
		private XsollaApi 			api;// "api":{}

		public string GetAcceessToken() {
			return accessToken;
		}

		public XsollaUser GetUser()
		{
			return user;
		}


		public XsollaProject GetProject()
		{
			return project;
		}

		public XsollaPurchase GetPurchase()
		{
			return purchase;
		}

		public XsollaSettings GetSettings()
		{
			return settings;
		}

		public XsollaTranslations GetTranslations()
		{
			return translations;
		}

		public void SetAccessToken(string pToken)
		{
			accessToken = pToken;
		}


		public IParseble Parse (JSONNode utilsNode)
		{
			//accessToken 	= utilsNode [XsollaApiConst.ACCESS_TOKEN].Value;
			user 			= new XsollaUser ().Parse (utilsNode [XsollaApiConst.R_USER]) as XsollaUser;
			project 		= new XsollaProject ().Parse (utilsNode [XsollaApiConst.R_PROJECT]) as XsollaProject;
			purchase 		= new XsollaPurchase ().Parse (utilsNode [XsollaApiConst.R_PURCHASE]) as XsollaPurchase;
			settings 		= new XsollaSettings ().Parse (utilsNode [XsollaApiConst.R_SETTINGS]) as XsollaSettings;
			translations 	= new XsollaTranslations ().Parse (utilsNode [XsollaApiConst.R_TRANSLATIONS]) as XsollaTranslations;
			api 			= new XsollaApi ().Parse (utilsNode [XsollaApiConst.R_API]) as XsollaApi;

			return this;
		}


		public override string ToString ()
		{
			return string.Format ("[XsollaUtils] " 
			                      + "\n\n user {0}"
			                      + "\n\n project {1}"
			                      + "\n\n purchase {2}"
			                      + "\n\n settings {3}"
			                      + "\n\n project {4}"
			                      + "\n\n api {5}",
			                      user, project, purchase, settings, project, api);
		}
	}
}

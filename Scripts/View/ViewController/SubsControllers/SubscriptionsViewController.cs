using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace Xsolla
{
	public class SubscriptionsViewController: MonoBehaviour
	{
		XsollaSubscriptions 	_listSubs;
		public Text 			_titlePage;
		public GameObject 		_listSubsView;
		public Text 			_timeAlert;
		public MyRotation		mProgressBar;

		private XsollaUtils		mUtils;
		private const string mActiveSubsUrl = "paystation2/api/recurring/active";

		private const string PREFAB_SPEC_SUBS = "Prefabs/SimpleView/_ScreenShop/ShopItemSubscriptionSpecial";

		public void init(XsollaUtils pUtils)
		{
			mUtils = pUtils;
			_titlePage.text = (pUtils.GetProject().components ["subscriptions"].Name != "") ? pUtils.GetProject().components["subscriptions"].Name : pUtils.GetTranslations().Get(XsollaTranslations.SUBSCRIPTION_PAGE_TITLE); 
			mProgressBar.SetLoading(false);
			GetSubsRequest();
		}

		private void GetSubsRequest()
		{
			mProgressBar.SetLoading(true);
			// получить список пакетов
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			ApiRequest.Instance.getApiRequest(new XsollaRequestPckg(mActiveSubsUrl, lParams), ActiveSubsRecived, ErrorRecived);
		}

		private void ErrorRecived(XsollaErrorRe pError)
		{
		}

		private void ActiveSubsRecived(JSONNode pNode)
		{
			mProgressBar.SetLoading(false);
			XsollaSubscriptions subs = new XsollaSubscriptions().Parse(pNode) as XsollaSubscriptions;
			subs.GetItemsList().ForEach((item) =>
				{
					AddSubs(item);
				});
		}


		private void AddSubs(XsollaSubscription pSub)
		{
			GameObject subObj = Instantiate(Resources.Load(PREFAB_SPEC_SUBS)) as GameObject;
			subObj.transform.SetParent(_listSubsView.transform);
			Resizer.SetDefScale(subObj);
			SubscriptionBtnController controller = subObj.GetComponent<SubscriptionBtnController>();
			controller.InitBtn(pSub,mUtils.GetTranslations());
		}
	}
}


using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla
{
	public class SubManagerDetailPaymentPartController: MonoBehaviour
	{
		public Text mPartTitle;
		public GameObject mDetailsContainer;
		public GameObject mUnLinkBtn;

		private const String mLabeltextPrefab = "Prefabs/Screens/SubsManager/Simple/DetailLabelValue";
		private XsollaUtils mUtils;
		private XsollaManagerSubDetails mSubDetail;

		public Button getUnlinkBtn()
		{
			return mUnLinkBtn.GetComponent<Button>();
		}

		public void init(XsollaManagerSubDetails pSubDetail, XsollaUtils pUtils, Action<XsollaManagerSubDetails> pLinkAction)
		{
			mUtils = pUtils;
			mSubDetail = pSubDetail;
			mPartTitle.text = pUtils.GetTranslations().Get("user_subscription_payment_title");

			if (pSubDetail.mPaymentMethodType != "notify" && pSubDetail.mPaymentMethodName != "")
				mUnLinkBtn.GetComponent<Text>().text = pUtils.GetTranslations().Get("user_subscription_unlink_payment_account");
			else
				mUnLinkBtn.GetComponent<Text>().text = pUtils.GetTranslations().Get("user_subscription_add");
			getUnlinkBtn().onClick.AddListener(() => pLinkAction(pSubDetail));
		
			// добавляем поля датализации 
			List<LabelValue> listFileds = getImportDetails();
			foreach (LabelValue item in listFileds)
			{
				GameObject obj = Instantiate(Resources.Load(mLabeltextPrefab)) as GameObject;
				LabelValueController controller = obj.GetComponent<LabelValueController>();
				controller.init(item.label, item.value, item.actionLabel, item.action);
				obj.transform.SetParent(mDetailsContainer.transform);
			}
		}
			
		public List<LabelValue> getImportDetails()
		{
			List<LabelValue> list = new List<LabelValue>();
			XsollaTranslations translation = mUtils.GetTranslations();
			// ПОЛЯ ДЕТАЛИЗАЦИИ
			// имя
			if (mSubDetail.mPaymentMethodName != "null")
				list.Add(new LabelValue(translation.Get("user_subscription_payment_method"), mSubDetail.mPaymentMethodName + " (" + mSubDetail.mPaymentMethodVisName + ")"));
			// сумма след списание
			list.Add(new LabelValue(translation.Get("user_subscription_next_bill_sum"), mSubDetail.mNextCharge.ToString()));
			// дата след списание
			list.Add(new LabelValue(translation.Get("user_subscription_next_bill_date"), StringHelper.DateFormat(mSubDetail.mDateNextCharge)));
			return list;
		}

	}
}


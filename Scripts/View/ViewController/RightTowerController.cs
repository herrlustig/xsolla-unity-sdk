using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Xsolla 
{
	public class RightTowerController : MonoBehaviour 
	{

		private XsollaSummary _summary;
		private XsollaTranslations _translations;
		public GameObject orderSummaryPrefab;
		public GameObject summaryItemPrefab;
		public GameObject financeItemPrefab;
		public GameObject subTotalPrefab;
		public GameObject totalPrefab;
		public ImageLoader imageLoader;
		public LinearLayout linearLayout;

		public void InitView(XsollaTranslations translations, XsollaSummary summary)
		{
			_translations = translations;
			_summary = summary;

			if (this.gameObject.GetComponent<VerticalLayoutGroup>() == null)
			{
				this.gameObject.AddComponent<VerticalLayoutGroup>();
				VerticalLayoutGroup obj = this.GetComponent<VerticalLayoutGroup>();
				obj.childForceExpandHeight = false;
			}
			GameObject header = Instantiate (orderSummaryPrefab);
			header.GetComponentsInChildren<Text> () [0].text = translations.Get (XsollaTranslations.PAYMENT_SUMMARY_HEADER);
			linearLayout.AddObject(header);
			foreach(IXsollaSummaryItem purchase in _summary.GetPurchases ())
			{
				linearLayout.AddObject(GetSummaryItem(purchase));
			}
			XsollaFinance finance = _summary.GetFinance ();
			linearLayout.AddObject(GetItem(subTotalPrefab, translations.Get(XsollaTranslations.PAYMENT_SUMMARY_SUBTOTAL), PriceFormatter.Format(finance.subTotal.amount, finance.subTotal.currency)));
			if (finance.discount != null && finance.discount.amount > 0) 
			{
				linearLayout.AddObject(GetItem(financeItemPrefab, translations.Get(XsollaTranslations.PAYMENT_SUMMARY_DISCOUNT), "- " + PriceFormatter.Format(finance.discount.amount, finance.discount.currency)));
			}
			if (finance.fee != null) 
			{
				linearLayout.AddObject (GetItem (financeItemPrefab, translations.Get(XsollaTranslations.PAYMENT_SUMMARY_FEE), PriceFormatter.Format (finance.fee.amount, finance.fee.currency)));
			}
			if (finance.xsollaCredits != null && finance.xsollaCredits.amount > 0) 
			{
				linearLayout.AddObject(GetItem(financeItemPrefab, translations.Get(XsollaTranslations.PAYMENT_SUMMARY_XSOLLA_CREDITS), PriceFormatter.Format(finance.xsollaCredits.amount, finance.xsollaCredits.currency)));
			}
			linearLayout.AddObject(GetItem(totalPrefab, translations.Get(XsollaTranslations.PAYMENT_SUMMARY_TOTAL), PriceFormatter.Format(finance.total.amount, finance.total.currency)));
			if (finance.vat != null && finance.vat.amount > 0) 
			{
					linearLayout.AddObject(GetItem(financeItemPrefab, "VAT", PriceFormatter.Format(finance.vat.amount, finance.vat.currency)));
			}
			linearLayout.Invalidate ();
		}

		public void UpdateDiscont(XsollaTranslations pTranslation, XsollaSummary pSummary)
		{
			// clear all gameobject
			linearLayout.objects.ForEach((obj) => Destroy(obj));
			// redraw
			InitView(pTranslation, pSummary);

		}

		private GameObject GetSummaryItem(IXsollaSummaryItem purchase)
		{
			GameObject purchaseItemInstance = Instantiate(summaryItemPrefab) as GameObject;
			Image[] image = purchaseItemInstance.GetComponentsInChildren<Image>();
			string img = purchase.GetImgUrl ();
			if (!"".Equals (img))
				imageLoader.LoadImage (image [1], purchase.GetImgUrl ());
			else 
				image [1].gameObject.transform.parent.gameObject.SetActive (false);
			Text[] texts = purchaseItemInstance.GetComponentsInChildren<Text>();
			texts[0].text = purchase.GetName();
			texts[1].text = purchase.GetPrice();
			texts[2].text = purchase.GetDescription();
			texts[3].text = purchase.GetBonus();
			return purchaseItemInstance;
		}

		private GameObject GetItem(GameObject prefab, string title, object amount)
		{
			GameObject instance = Instantiate(prefab) as GameObject;
			Text[] texts = instance.GetComponentsInChildren<Text> ();
			texts [0].text = title;
			texts [1].text = amount.ToString();
			return instance;
		}

	}
}
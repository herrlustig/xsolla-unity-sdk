using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
	public class HistoryElemController: MonoBehaviour
	{
		public Text mDate;
		public Text mType;
		public Text mItem;
		public Text mBalance;
		public Text mPrice;
		public GameObject mSymbolRub;
		public GameObject mDevider;

		public String prepareTypeStr(String pStr)
		{
			String res = pStr;
			int i = 0;
			if (res.Contains("{{paymentName}}"))
			{
				res = res.Replace("{{paymentName}}", "{" + i + "}");
				i ++;
			}
			if (res.Contains("{{transactionId}}"))
			{
				res = res.Replace("{{transactionId}}", "{" + i + "}"); 	
				i ++;
			}
			if (res.Contains("{{comment}}"))
			{
				res = res.Replace("{{comment}}","{" + i + "}");
				i ++;
			}
			if (res.Contains("{{code}}"))
			{
				res = res.Replace("{{code}}","{" + i + "}");
				i ++;
			}
			return res;

		}

		public void Init(XsollaTranslations pTranslation, XsollaHistoryItem pItem, String pVirtCurrName,  Boolean pEven, Action pSortAction, Boolean pHeader = false, Boolean pDesc = true)
		{
			Image imgComp = this.GetComponent<Image>();
			imgComp.enabled = pEven;
	
			if (pHeader)
			{
				mDate.text = pTranslation.Get("balance_history_date") + (pDesc==true?" ▼":" ▲");
				Button sortBtn = mDate.gameObject.AddComponent<Button>();
				sortBtn.onClick.AddListener(delegate 
					{
						Logger.Log("On sort btn click");
						pSortAction();
						mDate.text = pTranslation.Get("balance_history_date") + " ↓";
					});

				mType.text = pTranslation.Get("balance_history_purpose");
				mItem.text = pTranslation.Get("balance_history_item");
				mBalance.text = pTranslation.Get("balance_history_vc_amount");
				mPrice.text = pTranslation.Get("balance_history_payment_amount");
				mPrice.alignment = TextAnchor.LowerLeft;

				// Activate devider 
				mDevider.SetActive(true);
				LayoutElement layout = this.transform.GetComponent<LayoutElement>();
				layout.minHeight = 30;
				return;
			}
				
			//mDate.text = pItem.date.ToShortDateString();
			mDate.text = pItem.date.ToString("MMM d, yyyy hh:mm tt");
			// balance_history_payment_info:"Payment via {{paymentName}}, transaction ID {{transactionId}}"
			// balance_history_payment_info_cancellation:"Refund. Payment via {{paymentName}}, transaction ID {{transactionId}}"
			// balance_history_subscription_change:"Subscription change. Payment via {{paymentName}}, transaction ID: {{transactionId}} "
			// balance_history_subscription_create:"Subscription create. Payment via {{paymentName}}, transaction ID {{transactionId}}"
			// balance_history_subscription_renew:"Subscription renewal. Payment via {{paymentName}}, transaction ID: {{transactionId}} "
			// balance_history_ingame_info:"In-Game Purchase"
			// balance_history_internal_info:"{{comment}}"
			// balance_history_coupon_info:"Coupon, code {{code}}"
			// subscription_cancellation:"Subscription cancellation"

			switch (pItem.operationType)
			{
			case "payment":
				{
					mType.text = String.Format(prepareTypeStr(pTranslation.Get("balance_history_payment_info")), pItem.paymentName, pItem.invoiceId);
					break;
				}
			case "cancellation":
				{
					mType.text = String.Format(prepareTypeStr(pTranslation.Get("balance_history_payment_info_cancellation")), pItem.paymentName, pItem.invoiceId);
					break;
				}
			case "inGamePurchase":
				{
					mType.text = String.Format(prepareTypeStr(pTranslation.Get("balance_history_ingame_info")));
					break;
				}
			case "internal":
				{
					mType.text = String.Format(prepareTypeStr(pTranslation.Get("balance_history_internal_info")), pItem.comment);
					break;
				}
			case "coupon":
				{
					mType.text = String.Format(prepareTypeStr(pTranslation.Get("balance_history_coupon_info")), pItem.couponeCode);
					break;
				}
			case "subscriptionRenew":
				{
					mType.text = String.Format(prepareTypeStr(pTranslation.Get("balance_history_subscription_renew")), pItem.paymentName, pItem.invoiceId);
					break;
				}
			case "subscriptionCreate":
				{
					mType.text = String.Format(prepareTypeStr(pTranslation.Get("balance_history_subscription_create")), pItem.paymentName, pItem.invoiceId);
					break;
				}
			case "subscriptionChange":
				{
					mType.text = String.Format(prepareTypeStr(pTranslation.Get("balance_history_subscription_change")), pItem.paymentName, pItem.invoiceId);
					break;
				}
			case "subscriptionCancellation":
				{
					mType.text = String.Format(prepareTypeStr(pTranslation.Get("subscription_cancellation")), pItem.paymentName, pItem.invoiceId);
					break;
				}
			default:
				{
					mType.text = "";
					break;
				}
			}
				
			if (pItem.virtualItems.items.GetCount() != 0)
				mItem.text = pItem.virtualItems.items.GetItemByPosition(0).GetName();

			if (pItem.vcAmount != 0)
				mBalance.text = ((pItem.vcAmount > 0)?"+":"") + pItem.vcAmount + " " + pVirtCurrName + "\n" + "(=" + pItem.userBalance + " " + pVirtCurrName + ")";
			else
				mBalance.text = "";

			if (pItem.paymentAmount != 0)
			{
				mPrice.text = CurrencyFormatter.FormatPrice(pItem.paymentCurrency, pItem.paymentAmount.ToString("0.00"));
				if (pItem.paymentCurrency == "RUB")
					mSymbolRub.SetActive(true);
				else
				{
					mSymbolRub.SetActive(false);
					mPrice.alignment = TextAnchor.LowerLeft;
				}
			}
			else
				mPrice.text = ""; 

		}
	}
}


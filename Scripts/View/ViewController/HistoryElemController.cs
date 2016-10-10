﻿using System;
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
		public GameObject mDevider;

		public void Init(XsollaTranslations pTranslation, XsollaHistoryItem pItem, Boolean pHeader = false)
		{
			if (pHeader)
			{
				mDate.text = pTranslation.Get("balance_history_date");
				mType.text = pTranslation.Get("balance_history_purpose");
				mItem.text = pTranslation.Get("balance_history_item");
				mBalance.text = pTranslation.Get("balance_history_vc_amount");
				mPrice.text = pTranslation.Get("balance_history_payment_amount");

				// Activate devider 
				mDevider.SetActive(true);

				return;
			}


			mDate.text = pItem.date.ToShortDateString();
			// balance_history_payment_info:"Payment via {{paymentName}}, transaction ID {{transactionId}}"
			// balance_history_payment_info_cancellation:"Refund. Payment via {{paymentName}}, transaction ID {{transactionId}}"
			// balance_history_subscription_change:"Subscription change. Payment via {{paymentName}}, transaction ID: {{transactionId}} "
			// balance_history_subscription_create:"Subscription create. Payment via {{paymentName}}, transaction ID {{transactionId}}"
			// balance_history_subscription_renew:"Subscription renewal. Payment via {{paymentName}}, transaction ID: {{transactionId}} "
			// balance_history_ingame_info:"In-Game Purchase"
			// balance_history_internal_info:"{{comment}}"
			// balance_history_coupon_info:"Coupon, code {{code}}"

			switch (pItem.operationType)
			{
			case "payment":
				{
					mType.text = String.Format(pTranslation.Get("balance_history_payment_info"), pItem.paymentName, pItem.invoiceId);
					break;
				}
			case "cancellation":
				{
					mType.text = String.Format(pTranslation.Get("balance_history_payment_info_cancellation"), pItem.paymentName, pItem.invoiceId);
					break;
				}
			case "inGamePurchase":
				{
					mType.text = String.Format(pTranslation.Get("balance_history_ingame_info"));
					break;
				}
			case "internal":
				{
					mType.text = String.Format(pTranslation.Get("balance_history_internal_info"), pItem.comment);
					break;
				}
			case "coupon":
				{
					mType.text = String.Format(pTranslation.Get("balance_history_coupon_info"), pItem.couponeCode);
					break;
				}
			case "subscriptionRenew":
				{
					mType.text = String.Format(pTranslation.Get("balance_history_subscription_renew"), pItem.paymentName, pItem.invoiceId);
					break;
				}
			case "subscriptionCreate":
				{
					mType.text = String.Format(pTranslation.Get("balance_history_subscription_create"), pItem.paymentName, pItem.invoiceId);
					break;
				}
			case "subscriptionChange":
				{
					mType.text = String.Format(pTranslation.Get("balance_history_subscription_change"), pItem.paymentName, pItem.invoiceId);
					break;
				}
			case "subscriptionCancellation":
				{
					mType.text = String.Format(pTranslation.Get("balance_history_payment_info_cancellation"), pItem.paymentName, pItem.invoiceId);
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

			mBalance.text = pItem.vcAmount + "\n" + "=" + pItem.userBalance;
			mPrice.text = pItem.paymentAmount.ToString();
		}
	}
}


using UnityEngine;
using System.Collections;

namespace Xsolla 
{
	public class PriceFormatter 
	{

		public static string Format(string amountString, string currency){
			var amount = float.Parse(amountString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
			switch (currency) {
			case "USD":
				amountString = string.Format("${0:0.00}", amount);
				break;
			case "EUR":
				amountString = string.Format("€{0:0.00}", amount);
				break;
			case "GBP":
				amountString = string.Format("£{0:0.00}", amount);
				break;
			case "BRL":
				amountString = string.Format("R${0:0.00}", amount);
				break;
			case "RUR":
			case "RUB":
				amountString = string.Format("{0:0.00}RUB", amount);
				break;//&#8399;
			default:
				amountString = string.Format("{0:0.00}", amount) + currency;
				break;//&#8399;
			}
			return amountString;
		}

		public static string Format(int amountInt, string currency){
			string amount = amountInt.ToString();
			return Format (amount, currency);
		}

		public static string Format(float amountFloat, string currency){
			string amount = amountFloat.ToString();
			return Format (amount, currency);
		}

	}
}


namespace Xsolla 
{
	public class CurrencyFormatter 
	{

		public static string FormatPrice(string currency, string amount){
			switch (currency) {
				case "USD":
					amount = "$" + amount;
					break;
				case "EUR":
					amount = "€" + amount;//&euro;
					break;
				case "GBP":
					amount = "£" + amount;//&pound;
					break;
				case "BRL":
					amount = "R$" + amount;
					break;
				case "RUB":
					amount = amount + "";
					break;//&#8399;
				default:
					amount = amount + " " + currency;
					break;//&#8399;
			}
			return amount;
		}
		
	}
}
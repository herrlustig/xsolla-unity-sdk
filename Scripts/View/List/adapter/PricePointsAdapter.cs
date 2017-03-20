

using UnityEngine;
using UnityEngine.UI;
using System;

namespace Xsolla 
{
	public class PricePointsAdapter : IBaseAdapter
	{
		private GameObject shopItemPrefab;

		public Action<float> OnBuyPricepoints;

		private XsollaPricepointsManager manager;
		private string _virtualCurrencyName = "Coins";
		private string _buyBtnText = "Buy";
		int current = 0;

		private ImageLoader imageLoader;

		public void Awake()
		{
			shopItemPrefab = Resources.Load("Prefabs/SimpleView/_ScreenShop/_ShopItemPricePoint") as GameObject;
		}

		public override int GetElementType(int id)
		{
			return 0;
		}

		public override int GetCount() 
		{
			return manager.GetCount ();
		}

		public XsollaPricepoint GetItem (int position)
		{
			return manager.GetItemByPosition (position);
		}

		public XsollaPricepoint GetItemById (int position)
		{
			return null;
		}

		public override GameObject GetView(int position)
		{
			GameObject shopItemInstance = Instantiate(shopItemPrefab) as GameObject;
			XsollaPricepoint pricepoint = GetItem (position);
			ShopItemViewAdapter itemAdapter = shopItemInstance.GetComponent<ShopItemViewAdapter>();
			itemAdapter.SetImage (pricepoint.GetImageUrl());
			itemAdapter.SetName (pricepoint.GetOutString());
			itemAdapter.SetDesc (_virtualCurrencyName);
			itemAdapter.SetBuyText (_buyBtnText);
			itemAdapter.SetSpecial (pricepoint.GetDescription());
			itemAdapter.SetPrice (pricepoint.GetPriceString());
			itemAdapter.SetLabel (pricepoint.GetAdvertisementType(), pricepoint.GetLabel());
			itemAdapter.SetOnClickListener(() => OnClickBuy((float)pricepoint.outAmount));
			return shopItemInstance;
		}

		private void OnClickBuy (float i){
			if (OnBuyPricepoints != null) 
			{
				OnBuyPricepoints(i);
			}
		}

		public override GameObject GetPrefab ()
		{
			return shopItemPrefab;
		}

		public void SetManager(XsollaPricepointsManager pricepoints)
		{
			manager = pricepoints;
		}

		public void SetManager(XsollaPricepointsManager pricepoints, string virtualCurrencyName, string buyBtnText)
		{
			_virtualCurrencyName = virtualCurrencyName;
			_buyBtnText = buyBtnText;
			SetManager(pricepoints);
		}

		public override GameObject GetNext ()
		{
			if (current < manager.GetCount ()) 
			{
				GameObject go = GetView (current);
				current ++;
				return go;
			}
			return null;
		}
	}
}

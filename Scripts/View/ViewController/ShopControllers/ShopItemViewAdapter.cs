using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Xsolla{

	public class ShopItemViewAdapter : MonoBehaviour {

		private ImageLoader imageLoader;
//		private Text[] textElements;
//		private int zero = 0;
		public GameObject ItemLable;
		public Text ItemLableText;
		public Image ItemImage;
		public Text ItemName;
		public Text ItemDesc;
		public Text ItemDescFull;
		public Text ItemBonus;
		public Text ItemPrice;
		public Text LearnMore;
		public Text TextBuy;
		public Toggle FavoriteToggle;
		public ColorController colorController;
		private Button button;

		void Awake(){
			button = GetComponentInChildren<Button> ();
		}

		public void SetLabel(AXsollaShopItem.AdType type, string text){
			StyleManager.BaseColor newColor = StyleManager.BaseColor.b_normal;
			switch(type){
				case XsollaShopItem.AdType.NONE:
					newColor = StyleManager.BaseColor.b_normal;
					break;
				case XsollaShopItem.AdType.SPECIAL_OFFER:
					newColor = StyleManager.BaseColor.b_special_offer;
					break;
				case XsollaShopItem.AdType.BEST_DEAL:
					newColor = StyleManager.BaseColor.b_best_deal;
					break;
				case XsollaShopItem.AdType.RECCOMENDED:
					newColor = StyleManager.BaseColor.b_recomended;
					break;
			}
			colorController.ChangeColor(0, newColor);
			colorController.ChangeColor(2, newColor);
			if (type == XsollaShopItem.AdType.NONE) {
				ItemLable.SetActive (false);
			} else {
				ItemLableText.text = text;
				ItemLable.SetActive (true);
			}
		}

		public void SetName(string coinsAmountText)
		{
			ItemName.text = coinsAmountText;
		}


		public void SetSpecial(string bonusText)
		{
			ItemBonus.text = bonusText;
		}

		public void SetDesc(string coinsText)
		{
			ItemDesc.text = coinsText;
		}

		public void SetFullDesc(string fullDesc)
		{
			if(fullDesc != null && !"".Equals(fullDesc))
				ItemDescFull.text = fullDesc;
			else
				ItemDescFull.transform.parent.transform.parent.gameObject.SetActive(false);
		}

		public void SetPrice(string price)
		{
			ItemPrice.text = price;
		}
		
		public void SetBuyText(string buyText) {
			TextBuy.text = buyText;
		}
	
		public void SetImage(string imgUrl)
		{
			if(imageLoader == null)
				imageLoader = GetComponentInChildren<ImageLoader> ();
			//image.UploadImageToCurrentView(imgUrl);
			if(imageLoader != null)
				imageLoader.imageUrl = imgUrl;
		}

		public void SetFavorite(bool isFavorite){
			if(FavoriteToggle != null)
				FavoriteToggle.isOn = isFavorite;
		}

		public void SetLoader(ImageLoader loader){
			imageLoader = loader;
		}

		public void SetOnClickListener(UnityAction onItemClick)
		{
			button.onClick.AddListener (onItemClick);
		}

		public void SetOnFavoriteChanged(UnityAction<bool> onValueChanged)
		{
			FavoriteToggle.onValueChanged.AddListener ((b) => onValueChanged(b));
		}



	}

}

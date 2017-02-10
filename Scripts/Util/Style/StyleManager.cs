using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections.Generic;
using System;

namespace Xsolla {
	public class StyleManager : Singleton<StyleManager> {

		public enum Themes // your custom enumeration
		{
			Black, 
			White
		};

		public enum BaseColor
		{
			bg_main,
			bg_top_menu,
			bg_footer,
			bg_left_menu,
			bg_shop_item,
			bg_item_btn,
			bg_pay_btn,
			bg_payment_method,
			b_recomended,
			b_best_deal,
			b_special_offer,
			b_normal,
			txt_top_menu,
			txt_left_menu,
			txt_footer,
			txt_title,
			txt_title_second,
			txt_item_name,
			txt_item_desc,
			txt_item_full_desc,
			txt_item_bonus,
			txt_paymen_system,
			txt_accent,
			txt_accent_2,
			txt_white,
			bg_input_field,
			b_input_field,
			bg_card_1,
			bg_card_2,
			bg_card_line,
			b_card,
			divider_1,
			divider_2,
			divider_3,
			selected,
			bonus,
			bg_ok,
			bg_error,
			link_on_notify
		};
		
		public Dictionary<BaseColor, Color32> colorsMap;
		public Themes CurrentTheme;  // this public var should appear as a drop down
		public Color32 invisColor;

		static BaseColor[] myColors = Enum.GetValues(typeof(BaseColor)) as BaseColor[];

		protected StyleManager() {}

		private void GetColors(){
			invisColor = new Color32(0,0,0,1);
			string theme;
			if (CurrentTheme == Themes.Black) {
				theme = "dark";
			} else {
				theme = "default";
			}
			TextAsset tempAsset = Resources.Load("Styles/theme") as TextAsset;
			string myString = tempAsset.text;
			JSONNode node = JSONNode.Parse (myString);
			JSONArray colorArray = node["theme"][theme]["colors"].AsArray;
			JSONArray colorNames = node["colors_map"].AsArray;
			colorsMap = new Dictionary<BaseColor, Color32>(colorArray.Count);
			for (int i = 0; i < colorArray.Count; i++) {
				int colorInt = Convert.ToInt32(colorArray[i].Value, 16);//colorArray[i].AsInt;//
				Color32 _color = ToColor(colorInt);
				colorsMap.Add(myColors[i], _color);
			}
		}

		public Color32 GetColor(BaseColor color){
			if (colorsMap != null && colorsMap.ContainsKey(color))
				return colorsMap[color];
			else
				return invisColor;
		}

		public Color32 ToColor(int HexVal)
		{
			byte R = (byte)((HexVal >> 16) & 0xFF);
			byte G = (byte)((HexVal >> 8) & 0xFF);
			byte B = (byte)((HexVal) & 0xFF);
			return new Color32(R, G, B, 255);
		}

		public void ChangeTheme(string newTheme){
			Logger.Log ("ChangeTheme " + newTheme);
			TextAsset tempAsset = Resources.Load("Styles/theme") as TextAsset;
			string myString = tempAsset.text;
			JSONNode node = JSONNode.Parse (myString);
			if ("dark".Equals (newTheme)) {
				CurrentTheme = Themes.Black;
			} else if ("default".Equals (newTheme)) {
				CurrentTheme = Themes.White;
			} else {
				newTheme = "dark";
			}
			JSONArray colorArray = node["theme"][newTheme]["colors"].AsArray;
			JSONArray colorNames = node["colors_map"].AsArray;
			colorsMap = new Dictionary<BaseColor, Color32>(colorArray.Count);
			for(int i = 0; i < colorArray.Count; i++){
				int colorInt = Convert.ToInt32(colorArray[i].Value, 16);//colorArray[i].AsInt;//
				Color32 _color = ToColor(colorInt);
				colorsMap.Add(myColors[i], _color);
			}
		}

		void Awake(){
			GetColors ();
		}
		// Use this for initialization
		void Start () {
		}

		// Update is called once per frame
		void Update () {
		
		}
	}
}

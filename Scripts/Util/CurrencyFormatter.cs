using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Text;

namespace Xsolla 
{
	public class CurrencyFormatter: Singleton<CurrencyFormatter> 
	{
		Dictionary<string, CurrencyFormat> mListCurrencyFormats;

		protected CurrencyFormatter() 
		{
		}

		void Awake()
		{
			mListCurrencyFormats = new Dictionary<string, CurrencyFormat>();
			LoadLib();
		}

		private void LoadLib()
		{
			TextAsset lTempAsset = Resources.Load("Styles/currency-format") as TextAsset;
			JSONNode lFullNode = JSONNode.Parse(lTempAsset.text);
			foreach(KeyValuePair<string, JSONNode> pair in lFullNode.AsObject.GetKeyValueDict())
			{
				mListCurrencyFormats.Add(pair.Key, new CurrencyFormat().Parse(pair.Value) as CurrencyFormat);
			}
			Logger.Log("Done parse CurrencyFormats");
		}

		public string FormatPrice(string pIso, decimal pAmount)
		{
			CurrencyFormat lFormat;
			if (!mListCurrencyFormats.TryGetValue(pIso.ToUpper(), out lFormat))
				Logger.LogError("Error get currency format from lib. Currency: " + pIso.ToUpper());

			return  lFormat.GetAmount(pIso, pAmount);
		}

		public string GetCurrencyGraphem(string pCurrency)
		{
			CurrencyFormat lFormat;
			if (!mListCurrencyFormats.TryGetValue(pCurrency.ToUpper(), out lFormat))
				Logger.LogError("Error get currency format from lib. Currency: " + pCurrency.ToUpper());

			return  lFormat.GetGraphem();
		}
	}

	public class CurrencyFormat: IParseble
	{
		private string mName;
		private int mFractionSize;
		private FormatSymbol mSymbol;
		private FormatSymbol mUniqSymbol;

		public string GetAmount(string pIso, decimal pAmount)
		{
			string res = mSymbol.Template();
			// Заменяем сумму
			res = res.Replace("1", pAmount.ToString("N" + mFractionSize));
			// Заменяем графему
			if ((mUniqSymbol == null) && (mSymbol == null))
				res = res.Replace("$", pIso);
			else	
				res = res.Replace("$", mUniqSymbol != null ? mUniqSymbol.Grapheme() : mSymbol.Grapheme());

			// переворачиваем если требует формат
			if (mSymbol.isRtl())
			{
				char[] charArray = res.ToCharArray();
				Array.Reverse(charArray);
				return new string(charArray);
			}

			return res;
		}

		public string GetGraphem()
		{
			return mSymbol.Grapheme();
		}

		public string GetUniqGraphem()
		{
			if (mUniqSymbol != null)
				return mUniqSymbol.Grapheme();
			else
				return "";
		}


		public IParseble Parse (JSONNode rootNode)
		{
			mName = rootNode["name"].Value;
			mFractionSize = rootNode["fractionSize"].AsInt;
			if (rootNode["symbol"].Value != "null")
				mSymbol = new FormatSymbol().Parse(rootNode["symbol"]) as FormatSymbol;
			if (rootNode["uniqSymbol"].Value != "null")
				mUniqSymbol = new FormatSymbol().Parse(rootNode["uniqSymbol"]) as FormatSymbol;
			return this;
		}

		public override string ToString ()
		{
			return string.Format ("[CurrencyFormat: mName={0}, mFractionSize={1}, mSymbol={2}, mUniqSymbol={3}]", mName, mFractionSize, mSymbol, mUniqSymbol);
		}
		
	}

	public class FormatSymbol: IParseble
	{
		private string mGrapheme;
		private string mTemplate;
		private bool mRtl;

		public string Template()
		{
			return mTemplate;
		}

		public string Grapheme()
		{
			return mGrapheme;
		}

		public bool isRtl()
		{
			return mRtl;
		}

		public IParseble Parse (JSONNode rootNode)
		{
			mGrapheme = rootNode["grapheme"].Value;
			mTemplate = rootNode["template"].Value;
			mRtl = rootNode[""].AsBool;
			return this;	
		}
	}

}
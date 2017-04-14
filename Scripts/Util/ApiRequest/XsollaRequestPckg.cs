using System.Collections.Generic;

namespace Xsolla
{
	public class XsollaRequestPckg
	{
		private bool mOnlyCache;
		private string mUrl;
		private Dictionary<string, object> mParams = null;

		/// <summary>
		/// Запрос будет только кэширован
		/// </summary>
		/// <returns></returns>
		public bool isOnlyCache()
		{
			return mOnlyCache;
		}

		/// <summary>
		/// Адрес запроса
		/// </summary>
		/// <returns></returns>
		public string Url()
		{
			return mUrl;
		}

		/// <summary>
		/// Набор параметров запроса
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, object> Params()
		{
			return mParams;
		}

		/// <summary>
		/// Создание пакета запроса
		/// </summary>
		/// <param name="pUrl">Адрес для запроса</param>
		/// <param name="pParams">Набор параметров для запроса</param>
		public XsollaRequestPckg(string pUrl, Dictionary<string, object> pParams, bool pOnlyCahce = false)
		{
			mUrl = pUrl;
			mParams = pParams != null ? pParams : new Dictionary<string, object>();
			mOnlyCache = pOnlyCahce;
		}

		/// <summary>
		/// Переоределение сравнения
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			// Если не сходятся адреса
			if (!this.Url().Equals((obj as XsollaRequestPckg).Url()))
				return false;

			// Если не сходятся кол-во параметров 
			if (this.Params().Count != (obj as XsollaRequestPckg).Params().Count)
				return false;

			// сравнение параметров 
			foreach(KeyValuePair<string, object> item in (obj as XsollaRequestPckg).Params())
			{
				if (Params().ContainsKey(item.Key))
				{
					if (!Params()[item.Key].Equals(item.Value))
						return false;
				}
				else
					return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return base.ToString();
		}

	}
}

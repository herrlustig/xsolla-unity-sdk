
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;namespace Xsolla {

	public class XsollaTranslations : IParseble
	{
		public static string API_ERROR_MESSAGE = "api_error_message";//"Sorry, we couldn't complete your request. Please contact our customer support."
		public static string APP_ERROR_TITLE = "app_error_title";//"Something is going wrong"
		public static string BACK_TO_LIST = "back_to_list";//"Back to payment methods"
		public static string BACK_TO_PAYMENT ="back_to_payment";//"Back to the payment"
		public static string BACK_TO_PICEPOINT = "back_to_pricepoint";//"Back to virtual currency"
		public static string BACK_TO_SAVEDMETHOD = "back_to_savedmethod";//"Back to payment accounts"
		public static string BACK_TO_SPECIALS = "back_to_specials";//"Back to Shop",
		public static string BACK_TO_SPECIALSLIST = "back_to_specialslist";//"Back to Shop",
		public static string BACK_TO_STORE = "back_to_store";//"Return to Store"
		public static string BACK_TO_SUBSCRIPTION = "back_to_subscription";//"Back to subscriptions"
		public static string BACK_TO_VIRTUALITEM = "back_to_virtualitem";//"Back to item shop"
		public static string BALANCE_BACK_BUTTON = "balance_back_button";//"Continue payment"
		public static string BALANCE_HISTORY_AMOUNT = "balance_history_amount";//"Amount"
		public static string BALANCE_HISTORY_COMMENT = "balance_history_comment";//"Comment"
		public static string BALANCE_HISTORY_DATE = "balance_history_date";//"Date"
		public static string BALANCE_HISTORY_NO_DATA = "balance_history_no_data";//"No payments yet"
		public static string BALANCE_HISTORY_PAGE_TITLE = "balance_history_page_title";//"Balance History"
		public static string BALANCE_HISTORY_PAYMENT_INFO = "balance_history_payment_info";//"Payment via {{paymentName}}, transaction ID {{transactionId}}"
		public static string BALANCE_HISTORY_REFRESH_BUTTON = "balance_history_refresh_button";//"Refresh"
		public static string BALANCE_HISTORY_STATUS = "balance_history_status";//"Status"
		public static string BALANCE_HISTORY_TRANSACTION_ID = "balance_history_transaction_id";//"Transaction ID {{id}}"
		public static string COUNTRY_CHANGE = "country_change";//"change"
		public static string FOOTER_AGREEMENT = "footer_agreement";//"User Agreement"
		public static string FOOTER_SECURED_CONNECTION = "footer_secured_connection";//"Secured Connection"
		public static string FORM_CC_CARD_NUMBER = "form_cc_card_number";//"Card number"
		public static string FORM_CC_EULA = "form_cc_eula";//"By submitting payment information you acknowledge that you have read and agree to be bound by the terms and conditions of the Xsolla <a href="http://xsolla.com/eula" target="_blank">End-User License Agreement</a>. Please public static string note = "note";//your credit card statement will public static string read = "read";//Xsolla USA Inc."
		public static string FORM_CC_EXP_DATE = "form_cc_exp_date";//"Expire Date"
		public static string FORM_CHECKOUT = "form_checkout";//"Checkout"
		public static string FORM_CHECKOUT_INTRO = "form_checkout_intro";//"To purchase {{itemDescription}} click "Pay now" and you will be redirected to the payment system"
		public static string FORM_CONTINUE = "form_continue";//"Pay now"
		public static string FORM_HEADER_CHANGE = "form_header_change";//"change"
		public static string FORM_NUMBER_2PAY = "form_number_2pay";//"2pay number"
		public static string FORM_NUMBER_XSOLLA = "form_number_xsolla";//"Xsolla number"
		public static string FORM_TOTAL = "form_total";//"Total"
		public static string LIST_OPTION_EXTRA = "list_option_extra";//"+{{percent}}%"
		public static string LIST_OPTION_OFF = "list_option_off";//"-{{percent}}%"
		public static string OFFER_BONUS = "offer_bonus";//"Bonus"
		public static string OFFER_DISCOUNT = "offer_discount";//"Discount"
		public static string OFFERS_COUNTDOWN_DAYS = "offers_countdown_days";//"days"
		public static string OFFERS_COUNTDOWN_LABEL = "offers_countdown_label";//"Happy hour! Ends in"
		public static string OFFERS_COUNTDOWN_METHODS = "offers_countdown_methods";//"Offers on selected payment methods"
		public static string OFFERS_DEFAULT_NAME = "offers_default_name";//"Happy hour!"
		public static string PAYMENT_INSTRUCTION_LABEL = "payment_instruction_label";//"How to pay"
		public static string PAYMENT_LIST_POPULAR_TITLE = "payment_list_popular_title";//"Popular Payment Methods"
		public static string PAYMENT_LIST_QUICK_TITLE = "payment_list_quick_title";//"Quick Payments"
		public static string PAYMENT_LIST_SEARCH = "payment_list_search";//"Search Payment Method"
		public static string PAYMENT_LIST_SEARCH_EG = "payment_list_search_eg";//"For example Visa"
		public static string PAYMENT_LIST_SEARCH_MOBILE = "payment_list_search_mobile";//"Search"
		public static string PAYMENT_LIST_SHOW_MORE = "payment_list_show_more";//"Show More Methods"
		public static string PAYMENT_LIST_SHOW_QUICK = "payment_list_show_quick";//"Back to Quick Methods"
		public static string PAYMENT_METHOD_NO_DATA = "payment_method_no_data";//"Payment option "{{method}}" not found. <br/>Please make sure you put the name right or try to use a different option."
		public static string PAYMENT_METHODS_PAGE_TITLE = "payment_methods_page_title";//"Payment Methods"
		public static string PAYMENT_PAGE_TITLE = "payment_page_title";//"Billing Information"
		public static string PAYMENT_PAGE_TITLE_VIA = "payment_page_title_via";//"Billing Information"
		public static string PAYMENT_SUMMARY_BONUS = "payment_summary_bonus";//"Bonus"
		public static string PAYMENT_SUMMARY_DISCOUNT = "payment_summary_discount";//"Discount"
		public static string PAYMENT_SUMMARY_FEE = "payment_summary_fee";//"Fee"
		public static string PAYMENT_SUMMARY_HEADER = "payment_summary_header";//"Order Summary"
		public static string PAYMENT_SUMMARY_SUBTOTAL = "payment_summary_subtotal";//"SubTotal"
		public static string PAYMENT_SUMMARY_TOTAL = "payment_summary_total";//"Total"
		public static string PAYMENT_SUMMARY_VAT = "payment_summary_vat";//"VAT included"
		public static string PAYMENT_SUMMARY_XSOLLA_CREDITS = "payment_summary_xsolla_credits";//"Xsolla Credits"
		public static string PAYMENT_SUMMARY_XSOLLA_CREDITS_HINT = "payment_summary_xsolla_credits_hint";//"Xsolla Credits description here"
		public static string PAYMENT_WAITING_BUTTON = "payment_waiting_button";//"Start again"
		public static string PAYMENT_WAITING_NOTICE = "payment_waiting_notice";//"Waiting for payment completion"
		public static string PERIOD_DAYS = "period_days";//"days"
		public static string PERIOD_MONTH1 = "period_month1";//"month"
		public static string PERIOD_MONTHS = "period_months";//"months"
		public static string PRICEPOINT_OPTION_BUTTON = "pricepoint_option_button";//"Buy Package"
		public static string PRICEPOINT_PAGE_TITLE = "pricepoint_page_title";//"Virtual Currency"
		public static string PRICEPOINT_PAGE_CUSTOM_AMOUNT_HIDE_TITLE = "custom_amount_hide_button";// custom_amount_hide_button:"Show Price Packages"
		public static string PRICEPOINT_PAGE_CUSTOM_AMOUNT_SHOW_TITLE = "custom_amount_show_button";// custom_amount_show_button:"Custom Virtual Currency Amount"
		public static string SAVED_METHODS_TITLE = "saved_methods_title";//"Your Saved Payment Methods"
		public static string SAVEDMETHOD_OTHER_ACCOUNT_LABEL = "savedmethod_other_account_label";//"Pay with another account"
		public static string SAVEDMETHOD_OTHER_LABEL = "savedmethod_other_label";//"Choose another payment method"
		public static string SAVEDMETHOD_OTHER_LABEL_MOBILE = "savedmethod_other_label_mobile";//"Choose Another Payment Method"
		public static string SAVEDMETHOD_PAGE_TITLE = "savedmethod_page_title";//"Saved Payment Methods"
		public static string STATE_NAME_INDEX = "state_name_index";//"Start again"
		public static string STATE_NAME_LIST = "state_name_list";//"Payment Methods"
		public static string STATE_NAME_PAYMENT = "state_name_payment";//"Payment"
		public static string STATE_NAME_PRICEPOINT = "state_name_pricepoint";//"Virtual Currency"
		public static string STATE_NAME_SAVEDMETHOD = "state_name_savedmethod";//"Saved Methods"
		public static string STATE_NAME_SUBSCRIPTION = "state_name_subscription";//"Subscriptions"
		public static string STATE_NAME_VIRTUALITEM = "state_name_virtualitem";//"Item Shop"
		public static string COUPON_PAGE_TITLE = "coupon_page_title"; //Redeem Coupon
		public static string COUPON_DESCRIPTION = "coupon_description"; //Please enter the code to redeem it for game items, currency or other bonuses
		public static string COUPON_CODE_TITLE = "coupon_code_title"; // Coupon Code
		public static string COUPON_CODE_EXAMPLE = "coupon_code_example";//Enter Code
		public static string COUPON_CONTROL_APPLY = "coupon_control_apply"; //Apply
		public static string STATUS_DONE_DESCRIPTION = "status_done_description";//"Successful purchase!"
		public static string STATUS_PAGE_TITLE = "status_page_title";//"Payment Status"
		public static string STATUS_PURCHASED_DESCRIPTION = "status_purchased_description";//"You purchased {{itemDescription}} for {{amount}}"
		public static string SUBSCRIPTION_MOBILE_PAGE_TITLE = "subscription_mobile_page_title";//"Subscriptions"
		public static string SUBSCRIPTION_PACKAGE_RATE = "subscription_package_rate";//"Charge {{amount}} every {{period}}"
		public static string SUBSCRIPTION_PACKAGE_RATE_MOBILE = "subscription_package_rate_mobile";//"per {{period}}"
		public static string SUBSCRIPTION_PAGE_TITLE = "subscription_page_title";//"Select Subscription"
		public static string SUPPORT_CONTACT_US = "support_contact_us";//"Contact Us!"
		public static string SUPPORT_CUSTOMER_SUPPORT = "support_customer_support";//"Customer support"
		public static string SUPPORT_LABEL = "support_label";//"Customer Support"
		public static string SUPPORT_NEED_HELP = "support_need_help";//"Need help?"
		public static string SUPPORT_PHONE = "support_phone";//"+1 877 797 65 52"
		public static string TOTAL = "total";//"Total:"
		public static string USER_BALANCE_LABEL = "user_balance_label";//"Balance"
		public static string VALIDATION_MESSAGE_CARD_MONTH = "validation_message_card_month";//"Expiration month is invalid"
		public static string VALIDATION_MESSAGE_CARD_YEAR = "validation_message_card_year";//"Expiration year is invalid"
		public static string VALIDATION_MESSAGE_CARDNUMBER = "validation_message_cardnumber";//"Please enter a valid credit card number"
		public static string VALIDATION_MESSAGE_CVV = "validation_message_cvv";//"CVV2/CVC2 is invalid"
		public static string VALIDATION_MESSAGE_EMAIL = "validation_message_email";//"Email is invalid"
		public static string VALIDATION_MESSAGE_MAX = "validation_message_max";//"Please enter a value less than or equal to {0}."
		public static string VALIDATION_MESSAGE_MAX_LENGTH = "validation_message_max_length";//"Please enter no more than {0} characters."
		public static string VALIDATION_MESSAGE_MIN = "validation_message_min";//"Please enter a value greater than or equal to {0}."
		public static string VALIDATION_MESSAGE_MIN_LENGTH = "validation_message_min_length";//"Please enter at least {0} characters."
		public static string VALIDATION_MESSAGE_PHONE = "validation_message_phone";//"Phone is invalid"
		public static string VALIDATION_MESSAGE_REQUIRED = "validation_message_required";//"Required"
		public static string VIRTUAL_ITEM_OPTION_BUTTON = "virtual_item_option_button";//"Buy"
		public static string VIRTUALSTATUS_DONE_DESCRIPTIONS = "virtualstatus_done_description";//"Successful purchase!"
		public static string VIRTUALITEM_NO_DATA = "virtualitem_no_data";//"No virtual items yet"
		public static string VIRTUALITEM_PAGE_TITLE = "virtualitem_page_title";//"Item Shop"
		public static string VIRTUALITEMS_TITLE_FAVORITE = "virtualitems_title_favorite";//"Favorite Items",
		public static string WHERE_IS_CPF_NAME = "where_is_cpf_name";//"Where to find your name"
		public static string WHERE_IS_CPF_NUMBER = "where_is_cpf_number";//"Where to find your registration number"
		public static string WHERE_IS_SECURITY_CODE = "where_is_security_code";//"Where to find your Security Code"
		public static string WHERE_IS_ZIP_POSTAL_CODE = "where_is_zip_postal_code";//"Enter in the first 5 numbers or letters of your zip/postal code"
		public static string XSOLLA_COPYRIGHT = "xsolla_copyright";//"© Xsolla, 2015"

		
		private Dictionary<string, string> translations;

		public XsollaTranslations()
		{
			translations = new Dictionary<string, string> ();
		}

		
		public string Get(string key) {
			if (translations.ContainsKey(key))
				return translations[key];
			return "";
		}

		public IParseble Parse (JSONNode translationsNode)
		{
			fillMap (translationsNode);
			return this;
		}

		private void fillMap(JSONNode translationsNode) {
			JSONClass jsonObj = translationsNode.AsObject;
			IEnumerator elements = jsonObj.GetEnumerator();
			while (elements.MoveNext()) {
				KeyValuePair<string, JSONNode> elem = (KeyValuePair<string, JSONNode>)elements.Current;
				translations.Add(elem.Key, elem.Value);
			}
		}

		public override string ToString ()
		{
			return string.Format ("[XsollaTranslations]");
		}
	
	}

}
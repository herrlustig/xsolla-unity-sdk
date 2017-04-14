# Xsolla Inc. Unity SDK VERSION 1.3.7

### GETTING STARTED

1. To become an Xsolla partner, sign up in Xsolla control panel at this address: https://publisher.xsolla.com/signup/?utm_source=unity&utm_medium=instruction  
2. Familiarize yourself with the Integration Guides and select the modules that best fit your project. You can find the Integration Guides here: https://developers.xsolla.com/#getting-started


**Features:**  
 - Saved payment methods;
 - Purchase of virtual currency;
 - Purchase of virtual items;
 - Purchase of subscriptions;
 - Promotions;
 - Redeem coupon;
 - User's payment history.
 - Saved payment methods management
 - Subscription management

• Set Up Instant Payment Notification (IPN): http://developers.xsolla.com/api.html#notifications  
• Create an Xsolla Access Token to conduct payments with maximum security. You can find documentation on creating a token here: http://developers.xsolla.com/api.html#payment-ui


##### Get Started

If you would like to accept payments through Xsolla’s payment UI, follow these steps:  
 1. Create empty GameObject on your game scen and add **XsollaSDK.cs** script on them. Or add script on any other object.
 2. To create Payment form you need call  
```cs
    CreatePaymentForm(token, actionOk(XsollaOkResult), actionError(XsollaError))  
```

|Member   | Description|
|------   | -----------|
|token    | Your purchase token ([Getting token method][3508ac7b])
|actionOk | Call when payment process completed, delegate here your func.<br>*Example: OnResulOkReceivied(XsollaOkResult result){Debug.Log(“Ok”);}* |
|actionError | Call when payment process canceled or some problems appeared, delegate here your func.<br>*Example: OnErrorReceivied(XsollaError data){Debug.Log(“Error”);}*|

[3508ac7b]: https://developers.xsolla.com/api.html#token "Getting token method"

 - Also you can use XsollaSDK.InitPaystation(string token) function to use our payment solution in native browser.

#### SDK RESPONSE OBJECTS

```cs
public class XsollaResult {
    public string invoice{ get; set;}
    public Status status{ get; set;}
    // DONE status mean successful payment
    public Dictionary<string, object> purchases;
    // purchases can contain
    // Key: «out» - virtual currency      | Value: int amount
    // Key: «id_package» - subscription   | Value: long subscriptionId
    // Key: «sku[itemId]» - virtual items | Value: int amount
}
```
```cs
public class XsollaError {
    public Source errorSource{ get; private set;}
    public int errorCode{ get; private set;}
    public string errorMessage{ get; private set;}
}
```

#### TRY IT!


You can look demo on https://livedemo.xsolla.com/sdk/unity/  
We have two test scenes in "XsollaUnitySDK" -> "Resources" -> "_Scenes" folder:  
- XsollaFarmFreshScene - emulates item shop  
- XollaTokenTestScene - here you can test your token.

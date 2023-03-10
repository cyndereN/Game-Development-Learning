using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Interfaces;
using UnityEngine.Purchasing.Models;

namespace UnityEngine.Purchasing.Interfaces
{
    interface IGooglePlayStoreService
    {
        void RetrieveProducts(ReadOnlyCollection<ProductDefinition> products, Action<List<ProductDescription>> onProductsReceived, Action onRetrieveProductFailed);
        void Purchase(ProductDefinition product);
        void Purchase(ProductDefinition product, Product oldProduct, int desiredProrationMode);
        void FinishTransaction(ProductDefinition product, string purchaseToken, Action<ProductDefinition, GooglePurchase, GoogleBillingResult, string> onConsume, Action<ProductDefinition, GooglePurchase, GoogleBillingResult> onAcknowledge);
        void FetchPurchases(Action<List<GooglePurchase>> onQueryPurchaseSucceed);
        void SetObfuscatedAccountId(string obfuscatedAccountId);
        void SetObfuscatedProfileId(string obfuscatedProfileId);
        void EndConnection();
        void ConfirmSubscriptionPriceChange(ProductDefinition product, Action<GoogleBillingResult> onPriceChangeAction);
    }
}

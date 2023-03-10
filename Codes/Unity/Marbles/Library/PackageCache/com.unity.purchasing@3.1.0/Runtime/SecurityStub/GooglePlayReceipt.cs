using System;

namespace UnityEngine.Purchasing.Security
{
	// See Google's reference docs.
	// http://developer.android.com/google/play/billing/billing_reference.html

    /// <summary>
    /// The state of the GooglePlay purchase.
    /// </summary>
	public enum GooglePurchaseState
    {
        /// <summary>
        /// The purchase was completed.
        /// </summary>
        Purchased,

        /// <summary>
        /// The purchase was cancelled.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The purchase was refunded.
        /// </summary>
        Refunded
	}

    /// <summary>
    /// A GooglePlay purchase receipt
    /// </summary>
	public class GooglePlayReceipt : IPurchaseReceipt
	{
        /// <summary>
        /// The item's product identifier.
        /// </summary>
		public string productID { get; private set; }

        /// <summary>
        /// The ID  of the transaction. Obsolete use <c>orderID</c>
        /// </summary>
		public string transactionID { get; private set; }

        /// <summary>
        /// A unique order identifier for the transaction.
        /// </summary>
		public string orderID { get; private set; }

        /// <summary>
        /// The package name of the app.
        /// </summary>
		public string packageName { get; private set; }

        /// <summary>
        /// A token that uniquely identifies a purchase for a given item and user pair.
        /// </summary>
		public string purchaseToken { get; private set; }

        /// <summary>
        /// The time the product was purchased, in milliseconds since the epoch (Jan 1, 1970).
        /// </summary>
		public DateTime purchaseDate { get; private set; }

        /// <summary>
        /// The purchase state of the order.
        /// </summary>
		public GooglePurchaseState purchaseState { get; private set; }

        /// <summary>
        /// Constructor that initializes the members from the input parameters.
        /// </summary>
        /// <param name="productID"> The item's product identifier. </param>
        /// <param name="transactionID"> The ID  of the transaction. Obsolete. </param>
        /// <param name="packageName"> The package name of the app. </param>
        /// <param name="purchaseToken"> The token that uniquely identifies a purchase for a given item and user pair. </param>
        /// <param name="purchaseTime"> The time the product was purchased, in milliseconds since the epoch (Jan 1, 1970). </param>
        /// <param name="purchaseState"> The purchase state of the order. </param>
		public GooglePlayReceipt(string productID, string transactionID, string packageName,
			string purchaseToken, DateTime purchaseTime, GooglePurchaseState purchaseState)
        {
			throw new NotImplementedException();
		}

        /// <summary>
        /// Constructor that initializes the members from the input parameters. Obsolete.
        /// </summary>
        /// <param name="productID"> The item's product identifier. </param>
        /// <param name="transactionID"> The ID  of the transaction. Obsolete, copies itself to the orderID as well. </param>
        /// <param name="orderID"> The unique order identifier for the transaction. </param>
        /// <param name="packageName"> The package name of the app. </param>
        /// <param name="purchaseToken"> The token that uniquely identifies a purchase for a given item and user pair. </param>
        /// <param name="purchaseTime"> The time the product was purchased, in milliseconds since the epoch (Jan 1, 1970). </param>
        /// <param name="purchaseState"> The purchase state of the order. </param>
		[Obsolete("Use variant with string orderID in signature")]
		public GooglePlayReceipt(string productID, string transactionID, string orderID, string packageName,
			string purchaseToken, DateTime purchaseTime, GooglePurchaseState purchaseState)
        {
			throw new NotImplementedException();
		}
	}
}

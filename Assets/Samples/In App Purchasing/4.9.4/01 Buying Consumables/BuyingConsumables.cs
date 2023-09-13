using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

namespace Samples.Purchasing.Core.BuyingConsumables
{
    public class BuyingConsumables : MonoBehaviour, IDetailedStoreListener
    {
        IStoreController m_StoreController; // The Unity Purchasing system.

        //Your products IDs. They should match the ids of your products in your store.
        public const string goldProductId = "com.unity.gold1";
        public const string diamondProductId = "com.unity.diamond1";
        public const string removeAdsProductId = "com.unity.remove_ads";

        public Text GoldCountText;
        public Text DiamondCountText;
        public Image ImgAds;

        int m_GoldCount;
        int m_DiamondCount;

        void Start()
        {
            InitializePurchasing();
            UpdateUI();
        }

        void InitializePurchasing()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            //Add products that will be purchasable and indicate its type.
            builder.AddProduct(goldProductId, ProductType.Consumable);
            builder.AddProduct(diamondProductId, ProductType.Consumable);
            builder.AddProduct(removeAdsProductId, ProductType.NonConsumable);
            //builder.useCatalogProvider = true;
            UnityPurchasing.Initialize(this, builder);
        }

        public void BuyGold()
        {
            m_StoreController.InitiatePurchase(goldProductId);
        }

        public void BuyDiamond()
        {
            m_StoreController.InitiatePurchase(diamondProductId);
        }

        public void BuyRemoveAds()
        {
            m_StoreController.InitiatePurchase(removeAdsProductId);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("In-App Purchasing successfully initialized");
            m_StoreController = controller;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            OnInitializeFailed(error, null);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

            if (message != null)
            {
                errorMessage += $" More details: {message}";
            }

            Debug.Log(errorMessage);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            //Retrieve the purchased product
            var product = args.purchasedProduct;

            //Add the purchased product to the players inventory
            switch (product.definition.id)
            {
                case goldProductId:
                    {
                        AddGold();
                        break;
                    }
                case diamondProductId:
                    {
                        AddDiamond();
                        break;
                    }
                case removeAdsProductId:
                    {
                        HideAds();
                        break;
                    }
                default:
                    break;
            }

            Debug.Log($"Purchase Complete - Product: {product.definition.id}");

            //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
            return PurchaseProcessingResult.Complete;
        }

        private void HideAds()
        {
            ImgAds.gameObject.SetActive(false);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
                $" Purchase failure reason: {failureDescription.reason}," +
                $" Purchase failure details: {failureDescription.message}");
        }

        void AddGold()
        {
            m_GoldCount++;
            UpdateUI();
        }

        void AddDiamond()
        {
            m_DiamondCount++;
            UpdateUI();
        }

        void UpdateUI()
        {
            GoldCountText.text = $"Your Gold: {m_GoldCount}";
            DiamondCountText.text = $"Your Diamonds: {m_DiamondCount}";
        }
    }
}

﻿using UnityEngine;
//using AppodealAds.Unity.Api;
//using AppodealAds.Unity.Common;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;
    private BannerView bannerView;

    private void Awake()
    {
        Instance = this;   
    }

    private void Start()
    {
        /*******************************************************************************************************************************
         ADMOB
         ******************************************************************************************************************************/
#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            string adUnitId = "INSERT_IOS_AD_BANNER_ID_HERE";
#else
            string adUnitId = "unexpected_platform";
#endif

        MobileAds.Initialize(adUnitId);

        this.RequestBanner();

        /*******************************************************************************************************************************
         APPODEAL
         ******************************************************************************************************************************/

        //string appKey = "0981009c5520cd8c7c2152ffa0efb067798bf54f47fbdb7e"; //this is the app key appodeal gives each app you make
        //Appodeal.disableLocationPermissionCheck();
        //Appodeal.setTesting(true);
    }

    // ADMOB methods ---------------------------------------------------------------------------------
    private void RequestBanner()
    {
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        #elif UNITY_IPHONE
            string adUnitId = "INSERT_IOS_AD_BANNER_ID_HERE";
        #else
            string adUnitId = "unexpected_platform";
        #endif

        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        //// Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    //APPODEAL methods ------------------------------------------------------------------------------------
}

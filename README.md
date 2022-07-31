# Debts-Xamarin-App
Debts Financial Tracker - opensourced MvvmCross (6.4+) Xamarin app to track debts/loans/personal budget. 

This app used to be publicly available in Google Play / App Store but has been removed.
The APK can still be download here: https://apk.tools/details-debts-financial-manager-apk/

This app has been opensourced for Xamarin/MvvmCross learning purposes, commercial reusage (without my explict permission) is strictly forbidden.

# Running app

Source code has been stripped from API keys/App store ids.
AdMob app id/ads ids are public test ids (official id's for testing purpose). 

For proper running of app you have to include:
- google-services.json file for Android project and GoogleService-Info.plist file for iOS Project (firebase with deeplinking and AppInvite available)
- setup AdMob account (google ads)
- Fill Debts.iOS/Config/iOSAppConstants.cs and Debts.Droid/Config/AndroidAppConstants.cs file with:
    * AppCenter App Id *
    * AdMob ads ids *
- Fill AdMob constants on Android (Resources/Strings.xml)
- Fill GADApplicationIdentifier (AdMob) constants on iOS (Info.plist)
- Customize Share Deep Link Content - Services/AppGrowth/FireBaseShareLinkBuilderService on both platforms
- Setup App on Google Play / App Store and create premium products (+ update names/ids @ AppStoreBillingService.cs class on iOS / GoogleBillingClient.cs class on Android)

- Find in project & Uncomment "UNCOMMENT THIS WHEN YOU PROVIDE CONSTANTS AS DESCRIBED IN README FILE!!" when you go through above steps

The app will build&run (tested on Android API 30, iOS 15.0) regardless of above, but not all features will work as expected/correctly - deep links/share/go premium will be broken.

# App Video / iOS

See here: 
https://github.com/thefex/Debts-Xamarin-App/blob/main/Content/ios_showcase_video.mov


# App Store screenshots
![1](https://raw.githubusercontent.com/thefex/Debts-Xamarin-App/main/Content/1.webp)
![2](https://raw.githubusercontent.com/thefex/Debts-Xamarin-App/main/Content/2.webp)
![3](https://raw.githubusercontent.com/thefex/Debts-Xamarin-App/main/Content/3.webp)
![4](https://raw.githubusercontent.com/thefex/Debts-Xamarin-App/main/Content/4.webp)
![5](https://raw.githubusercontent.com/thefex/Debts-Xamarin-App/main/Content/5.webp)
![6](https://raw.githubusercontent.com/thefex/Debts-Xamarin-App/main/Content/6.webp)
![7](https://raw.githubusercontent.com/thefex/Debts-Xamarin-App/main/Content/7.webp)
![8](https://raw.githubusercontent.com/thefex/Debts-Xamarin-App/main/Content/8.webp)

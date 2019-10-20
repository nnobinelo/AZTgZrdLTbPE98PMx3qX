<<<<<<< HEAD
using System.Collections;
using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android {
	public class AppodealRewardedVideoCallbacks
#if UNITY_ANDROID
		: AndroidJavaProxy {
			IRewardedVideoAdListener listener;

			internal AppodealRewardedVideoCallbacks (IRewardedVideoAdListener listener) : base ("com.appodeal.ads.RewardedVideoCallbacks") {
				this.listener = listener;
			}

			void onRewardedVideoLoaded (bool precache) {
				listener.onRewardedVideoLoaded (precache);
			}

			void onRewardedVideoFailedToLoad () {
				listener.onRewardedVideoFailedToLoad ();
			}

			void onRewardedVideoShown () {
				listener.onRewardedVideoShown ();
			}

			void onRewardedVideoFinished (double amount, AndroidJavaObject name) {
				listener.onRewardedVideoFinished (amount, null);
			}

			void onRewardedVideoFinished (double amount, string name) {
				listener.onRewardedVideoFinished (amount, name);
			}

			void onRewardedVideoClosed (bool finished) {
				listener.onRewardedVideoClosed (finished);
			}

			void onRewardedVideoExpired () {
				listener.onRewardedVideoExpired ();
			}
			void onRewardedVideoClicked () {
				listener.onRewardedVideoClicked ();
			}

		}
#else
	{
		public AppodealRewardedVideoCallbacks (IRewardedVideoAdListener listener) { }
=======
using UnityEngine;
using System.Collections;
using AppodealAds.Unity.Common;

namespace AppodealAds.Unity.Android 
{
	public class AppodealRewardedVideoCallbacks 
#if UNITY_ANDROID
		: AndroidJavaProxy
	{
		IRewardedVideoAdListener listener;
		
		internal AppodealRewardedVideoCallbacks(IRewardedVideoAdListener listener) : base("com.appodeal.ads.RewardedVideoCallbacks") {
			this.listener = listener;
		}
		
		void onRewardedVideoLoaded(bool precache) {
            listener.onRewardedVideoLoaded(precache);
		}
		
		void onRewardedVideoFailedToLoad() {
			listener.onRewardedVideoFailedToLoad();
		}
		
		void onRewardedVideoShown() {
			listener.onRewardedVideoShown();
		}

        void onRewardedVideoFinished(double amount, AndroidJavaObject name) {
			listener.onRewardedVideoFinished(amount, null);
		}
		
        void onRewardedVideoFinished(double amount, string name) {
			listener.onRewardedVideoFinished(amount, name);
		}
		
		void onRewardedVideoClosed(bool finished) {
			listener.onRewardedVideoClosed(finished);
		}

        void onRewardedVideoExpired(){
            listener.onRewardedVideoExpired();
        }

	}
#else
	{
		public AppodealRewardedVideoCallbacks(IRewardedVideoAdListener listener) { }
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
	}
#endif
}
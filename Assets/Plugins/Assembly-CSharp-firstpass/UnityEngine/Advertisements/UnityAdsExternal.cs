namespace UnityEngine.Advertisements
{
	internal static class UnityAdsExternal
	{
		private static UnityAdsPlatform impl;

		private static bool initialized;

		private static UnityAdsPlatform getImpl()
		{
			if (!initialized)
			{
				initialized = true;
				impl = new UnityAdsAndroid();
			}
			return impl;
		}

		public static void init(string gameId, bool testModeEnabled, string gameObjectName, string unityVersion)
		{
			getImpl().init(gameId, testModeEnabled, gameObjectName, unityVersion);
		}

		public static bool show(string zoneId, string rewardItemKey, string options)
		{
			return getImpl().show(zoneId, rewardItemKey, options);
		}

		public static void hide()
		{
			getImpl().hide();
		}

		public static bool isSupported()
		{
			return getImpl().isSupported();
		}

		public static string getSDKVersion()
		{
			return getImpl().getSDKVersion();
		}

		public static bool canShowZone(string zone)
		{
			return getImpl().canShowZone(zone);
		}

		public static bool hasMultipleRewardItems()
		{
			return getImpl().hasMultipleRewardItems();
		}

		public static string getRewardItemKeys()
		{
			return getImpl().getRewardItemKeys();
		}

		public static string getDefaultRewardItemKey()
		{
			return getImpl().getDefaultRewardItemKey();
		}

		public static string getCurrentRewardItemKey()
		{
			return getImpl().getCurrentRewardItemKey();
		}

		public static bool setRewardItemKey(string rewardItemKey)
		{
			return getImpl().setRewardItemKey(rewardItemKey);
		}

		public static void setDefaultRewardItemAsRewardItem()
		{
			getImpl().setDefaultRewardItemAsRewardItem();
		}

		public static string getRewardItemDetailsWithKey(string rewardItemKey)
		{
			return getImpl().getRewardItemDetailsWithKey(rewardItemKey);
		}

		public static string getRewardItemDetailsKeys()
		{
			return getImpl().getRewardItemDetailsKeys();
		}

		public static void setLogLevel(Advertisement.DebugLevel logLevel)
		{
			getImpl().setLogLevel(logLevel);
		}
	}
}

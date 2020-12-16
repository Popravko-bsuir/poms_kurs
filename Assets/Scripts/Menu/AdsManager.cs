using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace Menu
{
    public class AdsManager : MonoBehaviour, IUnityAdsListener
    {
        private string _googlePlayId = "3935979";
        private string _placementId = "rewardedVideo";
        private bool _isTestMode = false;
        public UnityEvent onAddFinish;
        public UnityEvent onAddSkipp;
        public UnityEvent onAddFailed;

        private void Start()
        {
            Advertisement.AddListener(this);
            Advertisement.Initialize(_googlePlayId, _isTestMode);
        }

        public void ShowBanner()
        {
            Advertisement.Show();
        }

        public void ShowVideoAd()
        {
            if (Advertisement.IsReady(_placementId))
            {
                Advertisement.Show(_placementId);
            }
        }

        public void OnUnityAdsReady (string placementId) 
        {
            if (placementId == _placementId)
            {
                
            }
        }

        public void OnUnityAdsDidFinish (string placementId, ShowResult showResult) 
        {
            if (showResult == ShowResult.Finished) 
            {
                Debug.Log("Nais");
                onAddFinish.Invoke();
            } 
            else if (showResult == ShowResult.Skipped) 
            {
                Debug.Log("Ne Nais");
                onAddSkipp.Invoke();
            } 
            else if (showResult == ShowResult.Failed) 
            {
                Debug.Log("Ne Nais");
                onAddFailed.Invoke();
            }
        }

        public void OnUnityAdsDidError (string message) 
        {
        }

        public void OnUnityAdsDidStart (string placementId) 
        {
        } 
    }
}

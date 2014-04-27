using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Locations;
using Android.Provider;
using POIApp.Lib;

namespace POIApp
{
  static class GlobalApp
  {
    public static readonly string TAG = "POIApp";
    // Request Codes
    public static readonly int REQUEST_CODE_LOCATION_SOURCE_SETTINGS = 1;
  }

  [Activity(Label = "POIs", MainLauncher = true)]
  public class POIListActivity : Activity, ILocationListener
  {
    // Private Members    
    private ListView _poiListView;
    private POIListViewAdapter _adapter;
    private LocationManager _locMgr;

    private bool _isEnableGpsRequested = false;

    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //LifeCycle
    protected override void OnCreate(Bundle bundle)
    {
      //Call SuperClass OnCreate
      base.OnCreate(bundle);
      //Log.Info(GlobalApp.TAG, "OnCreate()");

      // Set our view from the "POIList" layout resource
      SetContentView(Resource.Layout.POIList);

      // request an LocationService instance
      _locMgr = GetSystemService(Context.LocationService) as LocationManager;

      // Hooking up POIListViewAdapter
      _poiListView = FindViewById<ListView>(Resource.Id.poiListView);
      _adapter = new POIListViewAdapter(this);
      _poiListView.Adapter = _adapter;

      //hook _poiListView up the event handler.
      _poiListView.ItemClick += POIClicked;
    }

    // Called after the activity has been stopped, just prior to it being started again. Always followed by onStart()
    protected override void OnRestart()
    {
      base.OnRestart();
      //Log.Info(GlobalApp.TAG, "OnRestart()");
    }

    // The activity is about to become visible.
    protected override void OnStart()
    {
      base.OnStart();
      //Log.Info(GlobalApp.TAG, "OnStart()");
    }

    // The activity has become visible (it is now "resumed").
    protected override void OnResume()
    {
      base.OnResume();
      //Log.Info(GlobalApp.TAG, "OnResume()");

      //Notify BaseAdapter<> of Data Changes
      _adapter.NotifyDataSetChanged();

      //If LocationManager ProviderEnabled
      if (_locMgr.IsProviderEnabled(LocationManager.GpsProvider))
      {
        // Specifying the criteria for the desired Location Provider
        Criteria criteria = new Criteria();
        criteria.Accuracy = Accuracy.NoRequirement;
        criteria.PowerRequirement = Power.NoRequirement;
        string provider = _locMgr.GetBestProvider(criteria, true);
        try
        {
          _locMgr.RequestLocationUpdates(provider, 20000, 100, this);
        }
        catch (Exception e)
        {
          Log.Info(GlobalApp.TAG, String.Format("RequestLocationUpdate Exception: {0}", e));
        }
      }
      //Request if user Wants to enable GPS
      else
      {
        if (!_isEnableGpsRequested)
        {
          //Enable it, only ask one time, in onCreate
          _isEnableGpsRequested = true;
          Utils.DialogEnableGPSProvider(this);
        }
      }
    }

    // Another activity is taking focus (this activity is about to be "paused").
    protected override void OnPause()
    {
      base.OnPause();
      //Log.Info(GlobalApp.TAG, "OnPause()");

      // Remove Location Provider Remove Updates
      _locMgr.RemoveUpdates(this);
    }

    // The activity is no longer visible (it is now "stopped")
    protected override void OnStop()
    {
      base.OnStop();
      //Log.Info(GlobalApp.TAG, "OnStop()");
    }

    // The activity is about to be destroyed.
    protected override void OnDestroy()
    {
      base.OnDestroy();
      //Log.Info(GlobalApp.TAG, "OnDestroy()");
    }

    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Save/Restore State
    protected override void OnSaveInstanceState(Bundle outState)
    {
      base.OnSaveInstanceState(outState);

      Log.Info(GlobalApp.TAG, String.Format("OnSaveInstanceState _isEnableGpsRequested:[{0}]", _isEnableGpsRequested));
      outState.PutBoolean("isEnableGpsRequested", _isEnableGpsRequested);
    }

    protected override void OnRestoreInstanceState(Bundle savedInstanceState)
    {
      base.OnRestoreInstanceState(savedInstanceState);

      _isEnableGpsRequested = savedInstanceState.GetBoolean("isEnableGpsRequested");
      Log.Info(GlobalApp.TAG, String.Format("OnRestoreInstanceState _isEnableGpsRequested:[{0}]", _isEnableGpsRequested));
    }

    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Menu
    // The OnCreateOptionsMenu() method is called to give an opportunity 
    // to the Activity parameter to define actions for the ActionBar.
    public override bool OnCreateOptionsMenu(IMenu menu)
    {
      MenuInflater.Inflate(Resource.Menu.POIListViewMenu, menu);
      return base.OnCreateOptionsMenu(menu);
    }

    public override bool OnOptionsItemSelected(IMenuItem item)
    {
      switch (item.ItemId)
      {
        case Resource.Id.actionNew:
          StartActivity(typeof(POIDetailActivity));
          return true;

        case Resource.Id.actionRefresh:
          // The POIJsonService.RefreshCache() method is called to refresh the 
          // cache with the *.json files stored locally on the device.
          POIData.Service.RefreshCache();
          // The _adapter.NotifyDataSetChanged method is called so that poiListView
          // will be refreshed based on the updated cache of POIs.
          _adapter.NotifyDataSetChanged();
          return true;

        default:
          return base.OnOptionsItemSelected(item);
      }
    }

    //Process ListView Item Clicks
    protected void POIClicked(object sender, ListView.ItemClickEventArgs e)
    {
      //PointOfInterest poi = POIData.Service.GetPOI((int) e.Id);
      //Console.WriteLine("POIClicked: Id: {0}, Name: {1}", poi.Id, poi.Name);

      // setup the intent to pass the POI id to the detail view
      Intent poiDetailIntent = new Intent(this, typeof(POIDetailActivity));
      poiDetailIntent.PutExtra("poiId", (int)e.Id);
      StartActivity(poiDetailIntent);
    }

    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Activity Result
    //http://docs.xamarin.com/recipes/android/fundamentals/activity/start_activity_for_result/
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
    {
      base.OnActivityResult(requestCode, resultCode, data);
      
      Log.Info(GlobalApp.TAG, String.Format("onActivityResult: requestCode:[{0}], resultCode:[{1}], data:[{2}]", requestCode, resultCode, data));

      if (requestCode == GlobalApp.REQUEST_CODE_LOCATION_SOURCE_SETTINGS && resultCode == 0)
      {
        String provider = Settings.Secure.GetString(this.ContentResolver, Settings.Secure.LocationProvidersAllowed);
        if (provider != null)
        {
          Log.Info(GlobalApp.TAG, String.Format("onActivityResult: User enable : {0}", provider));
        }
        else
        {
          Log.Info(GlobalApp.TAG, String.Format("onActivityResult: User did not enable : {0}", provider));
        }
      }
    }

    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //ILocationListener implementation
    #region ILocationListener implementation

    public void OnLocationChanged(Location location)
    {
      // Set CurrentLocation on POIListViewAdapter when a location change is received
      _adapter.CurrentLocation = location;
      // call NotifyDataSetChange() to cause the ListView to be refreshed
      _adapter.NotifyDataSetChanged();
    }

    public void OnProviderDisabled(string provider)
    {
    }

    public void OnProviderEnabled(string provider)
    {
    }

    public void OnStatusChanged(string provider, Availability status, Bundle extras)
    {
    }

    #endregion
  }
}
using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace POIApp
{
  [Activity(Label = "POIs", MainLauncher = true)]
  public class POIListActivity : Activity
  {
    ListView _poiListView;
    POIListViewAdapter _adapter;

    protected override void OnCreate(Bundle bundle)
    {
      //Call SuperClass OnCreate
      base.OnCreate(bundle);

      // Set our view from the "POIList" layout resource
      SetContentView(Resource.Layout.POIList);

      // Hooking up POIListViewAdapter
      _poiListView = FindViewById<ListView>(Resource.Id.poiListView);
      _adapter = new POIListViewAdapter(this);
      _poiListView.Adapter = _adapter;

      //hook _poiListView up the event handler.
      _poiListView.ItemClick += POIClicked;

      //Test use string resources
      //Android.Content.Res.Resources res = this.Resources;
      //Console.WriteLine(res.GetString(Resource.String.pathAppDataFolder));
    }

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
          // place holder for creating new poi
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
      PointOfInterest poi = POIData.Service.GetPOI((int) e.Id);
      Console.WriteLine("POIClicked: Id: {0}, Name: {1}", poi.Id, poi.Name);
    }
  }
}

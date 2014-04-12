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
    protected override void OnCreate(Bundle bundle)
    {
      //Call SuperClass OnCreate
      base.OnCreate(bundle);

      // Set our view from the "POIList" layout resource
      SetContentView(Resource.Layout.POIList);
    }
  }
}

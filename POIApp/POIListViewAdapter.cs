using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;
using Android.Util;

namespace POIApp
{
  public class POIListViewAdapter : BaseAdapter<PointOfInterest>
  {
    //Resources
    Android.Content.Res.Resources _r;
    //Store Context
    private readonly Activity _context;
    // The POIListActivity class will use this property to communicate location changes to the adapter
    public Location CurrentLocation { get; set; }

    public POIListViewAdapter(Activity context)
    {
      _context = context;

      //Get Resources from Context
      _r = _context.Resources;
    }

    #region BaseAdapter<PointOfInterest> implementation

    // abstract definition for read-only Count property.
    public override int Count
    {
      get { return POIData.Service.POIs.Count; }
    }

    // abstract definition index getter method
    public override PointOfInterest this[int position]
    {
      get { return POIData.Service.POIs[position]; }
    }

    // abstract definition for method that returns an int ID for a row in the data source. 
    public override long GetItemId(int position)
    {
      return POIData.Service.POIs[position].Id.Value;
    }

    // abstract definition for GetView(), which returns a view instance that represents a single row in the ListView item.
    // ...
    // The GetView() method is called for each row in the source dataset
    public override View GetView(int position, View convertView, ViewGroup parent)
    {
      // "inflating" a view from a layout file:
      // The first parameter of Inflate is a resource ID and the 
      // second is a root ViewGroup, which in this case can be left null 
      // since the view will be added to the ListView item when it is returned.
      // var view = _context.LayoutInflater.Inflate(Resource.Layout.POIListItem, null);

      // The GetView() method accepts a parameter named convertView, 
      // which is of type View. When a view is available for reuse, convertView will contain a reference to the view; 
      // otherwise, it will be null and a new view should be created.
      View view = convertView;
      if (view == null) view = _context.LayoutInflater.Inflate(Resource.Layout.POIListItem, null);

      // Populating row Views
      // Now that we have an instance of the view, 
      // we need to populate the fields. The View class defines a named FindViewById<T> method, 
      // which returns a typed instance of a widget contained in the view.
      PointOfInterest poi = POIData.Service.POIs[position];

      // The following code returns access to the nameTextView and sets the Text property:
      view.FindViewById<TextView>(Resource.Id.nameTextView).Text = poi.Name;

      // Populating addrTextView is slightly more complicated because we only want to use the portions of the address we have, 
      // and we want to hide the TextView if none of the address components are present.
      if (String.IsNullOrEmpty(poi.Address))
        //Gone Tells the parent ViewGroup to treat View as though it does not exist, so no space will be allocated in the layout.
        view.FindViewById<TextView>(Resource.Id.addrTextView).Visibility = ViewStates.Gone;
      else
        view.FindViewById<TextView>(Resource.Id.addrTextView).Text = poi.Address;

      // Calculate the distance between the CurrentLocation and a POI's location properties
      if ((CurrentLocation != null) && (poi.Latitude.HasValue) && (poi.Longitude.HasValue))
      {
        Location poiLocation = new Location("");
        // Get Values from Poi Data Service
        poiLocation.Latitude = poi.Latitude.Value;
        poiLocation.Longitude = poi.Longitude.Value;
        
        // Calculate Distance
        float distance = CurrentLocation.DistanceTo(poiLocation) / 1000;

        // Assign Calculated Distance to distanceTextView
        view.FindViewById<TextView>(Resource.Id.distanceTextView).Text = String.Format("{0:0,0.00} {1}", distance, _r.GetString(Resource.String.unitOfLengthKm));
        
        //Log.Info(GlobalApp.TAG, String.Format("distance: [{0}]", distance));
      }
      else
      {
        // Assign Unknown Distance to distanceTextView
        view.FindViewById<TextView>(Resource.Id.distanceTextView).Text = "??";
      }

      return view;
    }

    #endregion
  }
}
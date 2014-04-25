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
using Android.Util;

namespace POIApp
{
  [Activity(Label = "POIDetailActivity")]
  public class POIDetailActivity : Activity
  {
    // Private declarations 
    PointOfInterest _poi;
    // Bind Widgets to Private Variables
    EditText _nameEditText;
    EditText _descrEditText;
    EditText _addrEditText;
    EditText _latEditText;
    EditText _longEditText;
    ImageView _poiImageView;

    protected override void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);

      SetContentView(Resource.Layout.POIDetail);

      //bind private variables to user interface widget
      _nameEditText = FindViewById<EditText>(Resource.Id.nameEditText);
      _descrEditText = FindViewById<EditText>(Resource.Id.descrEditText);
      _addrEditText = FindViewById<EditText>(Resource.Id.addrEditText);
      _latEditText = FindViewById<EditText>(Resource.Id.latEditText);
      _longEditText = FindViewById<EditText>(Resource.Id.longEditText);
      _poiImageView = FindViewById<ImageView>(Resource.Id.poiImageView);

      // Private declarations PointOfInterest _poi;
      if (Intent.HasExtra("poiId"))
      {
        int poiId = Intent.GetIntExtra("poiId", -1);
        // Get Poi Details from Service
        _poi = POIData.Service.GetPOI(poiId);
        Log.Info(GlobalApp.TAG, String.Format("Update _poi.Id[{0}], _poi.Name[{1}]", _poi.Id, _poi.Name));
      }
      else
      {
        // Create a New Poi
        _poi = new PointOfInterest();
        Log.Info(GlobalApp.TAG, "Create a New POI");
      }

      // Updates User Interface Widgets with POI Data
      UpdateUI();
    }

    protected void UpdateUI()
    {
      _nameEditText.Text = _poi.Name;
      _descrEditText.Text = _poi.Description;
      _addrEditText.Text = _poi.Address;
      _latEditText.Text = _poi.Latitude.ToString();
      _longEditText.Text = _poi.Longitude.ToString();
    }

    public override bool OnCreateOptionsMenu(IMenu menu)
    {
      MenuInflater.Inflate(Resource.Menu.POIDetailMenu, menu);
      return base.OnCreateOptionsMenu(menu);
    }

    public override bool OnPrepareOptionsMenu(IMenu menu)
    {
      base.OnPrepareOptionsMenu(menu);

      // disable delete for a new POI
      if (!_poi.Id.HasValue)
      {
        IMenuItem item = menu.FindItem(Resource.Id.actionDelete);
        item.SetEnabled(false);
      }
      return true;
    }

    public override bool OnOptionsItemSelected(IMenuItem item)
    {
      switch (item.ItemId)
      {
        case Resource.Id.actionSave:
          SavePOI();
          return true;

        case Resource.Id.actionDelete:
          DeletePOI();
          return true;

        default: 
          return base.OnOptionsItemSelected(item);
      }
    }

    private void DeletePOI()
    {
      throw new NotImplementedException();
    }

    private void SavePOI()
    {
      throw new NotImplementedException();
    }
  }
}
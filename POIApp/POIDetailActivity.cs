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
using Android.Locations;
//using Android.Provider;

namespace POIApp
{
  [Activity(Label = "POIDetailActivity")]
  public class POIDetailActivity : Activity, ILocationListener
  {
    // Private declarations 
    PointOfInterest _poi;
    LocationManager _locMgr;

    // Bind Widgets to Private Variables
    EditText _nameEditText;
    EditText _descrEditText;
    EditText _addrEditText;
    EditText _latEditText;
    EditText _longEditText;
    ImageView _poiImageView;
    ImageButton _locationImageButton;
    ImageButton _mapImageButton;
    ImageButton _photoImageButton;

    ProgressDialog _progressDialog;
    bool _obtainingLocation = false;

    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //LifeCycle
    protected override void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);

      SetContentView(Resource.Layout.POIDetail);

      // Get LocationManager Reference
      _locMgr = GetSystemService(Context.LocationService) as LocationManager;

      // Bind private variables to user interface widget
      _nameEditText = FindViewById<EditText>(Resource.Id.nameEditText);
      _descrEditText = FindViewById<EditText>(Resource.Id.descrEditText);
      _addrEditText = FindViewById<EditText>(Resource.Id.addrEditText);
      _latEditText = FindViewById<EditText>(Resource.Id.latEditText);
      _longEditText = FindViewById<EditText>(Resource.Id.longEditText);
      _poiImageView = FindViewById<ImageView>(Resource.Id.poiImageView);
      _locationImageButton = FindViewById<ImageButton>(Resource.Id.locationImageButton);
      _mapImageButton = FindViewById<ImageButton>(Resource.Id.mapImageButton);
      _photoImageButton = FindViewById<ImageButton>(Resource.Id.photoImageButton);

      // Event Handlers
      _locationImageButton.Click += GetLocationClicked;
      _mapImageButton.Click += MapClicked;
      _photoImageButton.Click += NewPhotoClicked;

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

    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //LifeCycle
    protected void UpdateUI()
    {
      _nameEditText.Text = _poi.Name;
      _descrEditText.Text = _poi.Description;
      _addrEditText.Text = _poi.Address;
      _latEditText.Text = _poi.Latitude.ToString();
      _longEditText.Text = _poi.Longitude.ToString();
    }

    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Menu
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

    private void SavePOI()
    {
      if (Validate())
      {
        _poi.Name = _nameEditText.Text;
        _poi.Description = _descrEditText.Text;
        _poi.Address = _addrEditText.Text;
        _poi.Latitude = Double.Parse(_latEditText.Text);
        _poi.Longitude = Double.Parse(_longEditText.Text);

        POIData.Service.SavePOI(_poi);
        //Finish Activity
        Finish();
      }
    }

    private bool Validate()
    {
      bool isValid = true;

      if (String.IsNullOrEmpty(_nameEditText.Text))
      {
        _nameEditText.Error = this.Resources.GetString(Resource.String.msgValidatePOIName);
        isValid = false;
      }
      else
        _nameEditText.Error = null;

      double? tempLatitude = null;
      if (!String.IsNullOrEmpty(_latEditText.Text))
      {
        try
        {
          tempLatitude = Double.Parse(_latEditText.Text);
          if ((tempLatitude > 90) | (tempLatitude < -90))
          {
            _latEditText.Error = this.Resources.GetString(Resource.String.msgValidatePOILatitudeDecimalRange);
            isValid = false;
          }
          else
            _latEditText.Error = null;
        }
        catch
        {
          _latEditText.Error = this.Resources.GetString(Resource.String.msgValidatePOILatitudeDecimal);
          isValid = false;
        }
      }

      double? tempLongitude = null;
      if (!String.IsNullOrEmpty(_longEditText.Text))
      {
        try
        {
          tempLongitude = Double.Parse(_longEditText.Text);
          if ((tempLongitude > 180) | (tempLongitude < -180))
          {
            _longEditText.Error = this.Resources.GetString(Resource.String.msgValidatePOILongitudeDecimalRange);
            isValid = false;
          }
          else
            _longEditText.Error = null;
        }
        catch
        {
          _longEditText.Error = this.Resources.GetString(Resource.String.msgValidatePOILongitudeDecimal);
          isValid = false;
        }
      }

      return isValid;
    }

    protected void DeletePOI()
    {
      AlertDialog.Builder alertConfirm = new AlertDialog.Builder(this);
      alertConfirm.SetCancelable(false);
      //ConfirmDelete EventHandler
      alertConfirm.SetPositiveButton("OK", DeletePOIConfirm);
      //Empty event handler
      alertConfirm.SetNegativeButton("Cancel", delegate { });
      alertConfirm.SetMessage(String.Format(this.Resources.GetString(Resource.String.msgDialogPOIDelete), _poi.Name));
      alertConfirm.Show();
    }

    protected void DeletePOIConfirm(object sender, EventArgs e)
    {
      POIData.Service.DeletePOI(_poi);
      Toast toast = Toast.MakeText(this, String.Format(this.Resources.GetString(Resource.String.msgToastPOIDeleted), _poi.Name), ToastLength.Short);
      toast.Show();
      Finish();
    }

    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Button EventHandler
    //_locationImageButton.Click Event Handler
    protected void GetLocationClicked(object sender, EventArgs e)
    {
      _obtainingLocation = true;

      //Call Static Method
      _progressDialog = ProgressDialog.Show(this, "", this.Resources.GetString(Resource.String.msgDialogPOIGetLocation));

      Criteria criteria = new Criteria();
      criteria.Accuracy = Accuracy.NoRequirement;
      criteria.PowerRequirement = Power.NoRequirement;

      _locMgr.RequestSingleUpdate(criteria, this, null);
    }

    // _mapImageButton.Click += MapClicked EventHandler
    private void MapClicked(object sender, EventArgs e)
    {
    }

    //_photoImageButton.Click += NewPhotoClicked EventHandler
    private void NewPhotoClicked(object sender, EventArgs e)
    {
    }

    #region ILocationListener implementation

    public void OnLocationChanged(Location location)
    {
      _latEditText.Text = location.Latitude.ToString();
      _longEditText.Text = location.Longitude.ToString();

      // Reverse GeoCoding
      Geocoder geocdr = new Geocoder(this);
      IList<Address> addresses = geocdr.GetFromLocation(location.Latitude, location.Longitude, 5);

      if (addresses.Any())
      {
        UpdateAddressFields(addresses.First());
      }

      // Cancel Progress Dialog
      _progressDialog.Cancel();
      _obtainingLocation = false;
    }

    protected void UpdateAddressFields(Address addr)
    {
      if (String.IsNullOrEmpty(_nameEditText.Text))
        _nameEditText.Text = addr.FeatureName;

      if (String.IsNullOrEmpty(_addrEditText.Text))
      {
        for (int i = 0; i < addr.MaxAddressLineIndex; i++)
        {
          if (!String.IsNullOrEmpty(_addrEditText.Text))
            _addrEditText.Text += System.Environment.NewLine;
          _addrEditText.Text += addr.GetAddressLine(i);
        }
      }
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
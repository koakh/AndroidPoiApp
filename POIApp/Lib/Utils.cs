using Android.App;
using Android.Content;
using Android.Locations;
using Android.Provider;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POIApp.Lib
{
  public static class Utils
  {
    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Check if GPS Enabled
    public static bool IsGPSProviderEnabled(LocationManager pLocMgr)
    {
      if (pLocMgr.IsProviderEnabled(LocationManager.GpsProvider))
      {
        return true;
      }
      else
      {
        return false;
      }
    }
    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //Request if want to enable GPS(If Disabled) at onCreate
    public static void DialogEnableGPSProvider(Activity pContext)
    {
      Activity _context = pContext;

      AlertDialog.Builder dialogGPSProvider = new AlertDialog.Builder(pContext);
      dialogGPSProvider.SetCancelable(false);
      dialogGPSProvider.SetPositiveButton(_context.Resources.GetString(Resource.String.labelYes), delegate
      {
        Intent intent = new Intent(Settings.ActionLocationSourceSettings);
        _context.StartActivityForResult(intent, GlobalApp.REQUEST_CODE_LOCATION_SOURCE_SETTINGS);
      });
      dialogGPSProvider.SetNegativeButton(_context.Resources.GetString(Resource.String.labelNo), delegate { });
      dialogGPSProvider.SetMessage(String.Format(_context.Resources.GetString(Resource.String.msgDialogRequestEnableGPSProvider)));
      dialogGPSProvider.Show();
    }
    //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
  }
}

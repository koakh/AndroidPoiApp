using System;
using System.IO;
using Android.App;

namespace POIApp
{
  // Shared instance of IPOIDataService
  // single static field that is an instance of IPOIDataService, 
  // in this case the POIJsonService Implementation.
  public class POIData
  {
    // Singleton Service
    public static readonly IPOIDataService Service = new POIJsonService (
      Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "POIApp")
    );
  }
}

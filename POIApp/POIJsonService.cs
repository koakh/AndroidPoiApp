using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace POIApp
{
  public class POIJsonService : IPOIDataService
  {
    private string _storagePath;
    private List<PointOfInterest> _pois = new List<PointOfInterest>();

    //Constructor
    public POIJsonService(string storagePath)
    {
      //Init Parameters
      _storagePath = storagePath;
      // create the storage path if it does not exist 
      if (!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
    }

    #region IPOIDataService implementation
    public IReadOnlyList<PointOfInterest> POIs
    {
      get { return _pois; }
    }

    public void RefreshCache()
    {
      _pois.Clear();

      string[] filenames = Directory.GetFiles(_storagePath, "*.json");
      foreach (string filename in filenames)
      {
        string poiString = File.ReadAllText(filename);
        PointOfInterest poi = JsonConvert.DeserializeObject<PointOfInterest>(poiString);
        _pois.Add(poi);
      }
    }

    public PointOfInterest GetPOI(int id)
    {
      PointOfInterest poi = _pois.Find(p => p.Id == id); 
      return poi;
    }

    public void SavePOI(PointOfInterest poi)
    {
      Boolean newPOI = false;
      if (!poi.Id.HasValue)
      {
        poi.Id = GetNextId();
        newPOI = true;
      }

      // serialize POI
      string poiString = JsonConvert.SerializeObject(poi);
      // write new file or overwrite existing file
      File.WriteAllText(GetFilename(poi.Id.Value), poiString);

      // update cache if file save was successful
      // Note that we only need to add a POI to the cache when creating a new one and only after successfully writing the file.
      if (newPOI) _pois.Add(poi);
    }

    public void DeletePOI(PointOfInterest poi)
    {
      File.Delete(GetFilename(poi.Id.Value));
      _pois.Remove(poi);
    }
    #endregion

    #region Helper Functions
    private int GetNextId()
    {
      if (_pois.Count == 0)
        return 1;
      else
        return _pois.Max(p => p.Id.Value) + 1;
    }

    private string GetFilename(int id)
    {
      return Path.Combine(_storagePath, "poi" + id.ToString() + ".json");
    }
    #endregion
  }
}

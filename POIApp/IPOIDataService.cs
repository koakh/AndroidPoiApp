using System;
using System.Collections.Generic;

namespace POIApp
{
  public interface IPOIDataService
  {
    //The use of IReadOnlyList ensures that POIs cannot be added directly to the cache 
    //but must be added or deleted through the CRUD operations
    IReadOnlyList<PointOfInterest> POIs { get; } 
    void RefreshCache();
    PointOfInterest GetPOI(int id); 
    void SavePOI(PointOfInterest poi); 
    void DeletePOI(PointOfInterest poi);
  }
}

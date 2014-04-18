using System;
using System.IO;
using NUnit.Framework;

using POIApp;

namespace POITestApp
{
  [TestFixture]
  public class POITestFixture
  {
    IPOIDataService _poiService;

    [SetUp]
    public void Setup()
    {
      ///data/data/POITestApp.POITestApp/files/
      string storagePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

      //Initialize the private variable to an instance of POIJsonService
      _poiService = new POIJsonService(storagePath);

      // clear any existing json files
      foreach (string filename in Directory.EnumerateFiles(storagePath, "*.json"))
      {
        File.Delete(filename);
      }
    }

    [TearDown]
    public void Tear()
    {
    }

    [Test]
    public void CreatePOI()
    {
      PointOfInterest newPOI = new PointOfInterest();
      newPOI.Name = "New POI";
      newPOI.Description = "POI to test creating a new POI";
      newPOI.Address = "100 Main Street\nAnywhere, TX 75069";
      _poiService.SavePOI(newPOI);

      //Get id from New Created POI
      int testId = newPOI.Id.Value;

      // refresh the cashe to be sure the data was saved appropriately
      _poiService.RefreshCache();

      // verify the newly create POI exists, get it from poiId
      PointOfInterest poi = _poiService.GetPOI(testId);
      Assert.NotNull(poi);
      Assert.AreEqual(poi.Name, "New POI");
    }

    [Test]
    public void UpdatePOI()
    {
      PointOfInterest testPOI = new PointOfInterest();
      testPOI.Name = "Update POI";
      testPOI.Description = "POI being saved so we can test update";
      testPOI.Address = "100 Main Street\nAnywhere, TX 75069";
      _poiService.SavePOI(testPOI);

      //Get id from New Created POI
      int testId = testPOI.Id.Value;

      // refresh the cashe to be sure the data was 
      // poi was saved appropriately
      _poiService.RefreshCache();

      //Update POI
      PointOfInterest poi = _poiService.GetPOI(testId);
      poi.Description = "Updated Description for Update POI";
      _poiService.SavePOI(poi);

      // refresh the cashe to be sure the data was 
      // updated appropriately
      _poiService.RefreshCache();

      PointOfInterest findPOI = _poiService.GetPOI(testId);
      Assert.NotNull(findPOI);
      Assert.AreEqual(findPOI.Description, "Updated Description for Update POI");
    }

    [Test]
    public void DeletePOI()
    {
      PointOfInterest testPOI = new PointOfInterest();
      testPOI.Name = "Delete POI";
      testPOI.Description = "POI being saved so we can test delete";
      testPOI.Address = "100 Main Street\nAnywhere, TX 75069";
      _poiService.SavePOI(testPOI);

      //Get id from New Created POI
      int testId = testPOI.Id.Value;

      // refresh the cashe to be sure the data was 
      // poi was saved appropriately
      _poiService.RefreshCache();

      //Delete POI
      PointOfInterest deletePOI = _poiService.GetPOI(testId);
      Assert.IsNotNull(deletePOI);
      _poiService.DeletePOI(deletePOI);

      // refresh the cashe to be sure the data was 
      // deleted appropriately
      _poiService.RefreshCache();

      PointOfInterest findPOI = _poiService.GetPOI(testId);
      Assert.Null(findPOI);
    }
  }
}

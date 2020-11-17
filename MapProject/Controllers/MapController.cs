using MapProject.AWData;
using MapProject.Controllers.Interfaces;
using MapProject.Models;
using MapProject.POCOS.QueryData;
using MapProject.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;


namespace MapProject.Controllers
{
  public class MapController : Controller 
  {
    private MapProjectAWDBContext _context;
    private readonly IConversionServiceController _conversionServiceController;

    public MapController(MapProjectAWDBContext context, IConversionServiceController conversionServiceController)
    {
      _context = context;
      _conversionServiceController = conversionServiceController;
    }

    public ActionResult GetMapPage(string filter = null)
    {
        var mapViewModel = GetMap(filter);

      return View("MapView", mapViewModel);
    }

    public MapViewModel GetMap(string filter)
    {
      //IN = Individual (retail) customer --18,484  --yellow 
      //SP = Sales person -- 17 -- red
      //EM = Employee (non-sales) -- 273 --green
      //STORE = stores -- 701 --blue

      var address = _context.Address.ToList();
      var stateProvince = _context.StateProvince.ToList();
      var person = _context.Person.ToList();
      var businessEntityAddress = _context.BusinessEntityAddress.ToList();
      var store = _context.Store.ToList();

      var mapViewModel = new MapViewModel();

      var addressCounts = from p in person
                          group p by p.PersonType into personGroup
                          select new { PersonType = personGroup.Key, Count = personGroup.Count() };
      foreach (var type in addressCounts)
      {
        if (type.PersonType == "IN") { mapViewModel.INcount = type.Count; }
        if (type.PersonType == "SP") { mapViewModel.SPcount = type.Count; }
        if (type.PersonType == "EM") { mapViewModel.EMcount = type.Count; }
      }
      mapViewModel.STOREcount = store.Count();

      if (string.IsNullOrEmpty(filter))
      {

        var defaultList = new List<POCOS.QueryData.Location>();
        var tempModel = GetCoordinates(defaultList);
        mapViewModel.Addresses = tempModel.Addresses;
        mapViewModel.Markers = tempModel.Markers;

        return mapViewModel;
      }
      else
      { 
        var filteredAddresses = (from b in businessEntityAddress
                                 join s in store on b.BusinessEntityId equals s.BusinessEntityId into personStore
                                 from ps in personStore.DefaultIfEmpty()
                                 join p in person on b.BusinessEntityId equals p.BusinessEntityId into personStore2
                                 from ps2 in personStore2.DefaultIfEmpty()
                                 join a in address on b.AddressId equals a.AddressId
                                 join sp in stateProvince on a.StateProvinceId equals sp.StateProvinceId
                                 where ps2?.PersonType == ((filter == "ALL") ? ps2?.PersonType : (filter == "STORE") ? null : filter)
                                 orderby Guid.NewGuid()
                                 select new POCOS.QueryData.Location
                                 {
                                   personType = ((ps2?.PersonType == null) ? "STORE" : ps2?.PersonType),
                                   city = a.City,
                                   state = sp.Name,
                                   street = a.AddressLine1,
                                   name = ((filter == "STORE") ? ps?.Name : ps2?.FirstName + " " + ps2?.LastName)
                                 }
                               ).Take(10); //****!!! Just the first 10 while testing. The free API key only allows 15,000 transactions per month. There are over 19,000 addresses !!!****//;

        var tempModel = GetCoordinates(filteredAddresses.ToList());
        mapViewModel.Addresses = tempModel.Addresses;
        mapViewModel.Markers = tempModel.Markers;

        return mapViewModel;
      }
    }



    public MapViewModel GetCoordinates(List<POCOS.QueryData.Location> addressList)
    {

      //Toggle Microservice Architecure vs Monolithic Architecure
      var useMicroservice = true;

      var addressDisplayList = new List<POCOS.QueryData.Location>();

      if (useMicroservice)
      {
        //Decoupled service
        HttpClient client = new HttpClient();
        //client.BaseAddress = new Uri("https://localhost:44320/");
        client.BaseAddress = new Uri("https://addressconversionservice.azurewebsites.net/");
              
        var postResponse = client.PostAsJsonAsync("api/ConversionService/AddCoordinatesToAddresses", addressList);
        postResponse.Wait();
        var result = postResponse.Result;
        var readTask = result.Content.ReadAsAsync<List<POCOS.QueryData.Location>>();
        readTask.Wait();
        addressDisplayList = readTask.Result;
      }
      else
      {
        //Local service
        addressDisplayList = _conversionServiceController.AddCoordinatesToAddresses(addressList);
      }


      return new MapViewModel()
      {
        Addresses = addressDisplayList,
        Markers = JsonConvert.SerializeObject(addressDisplayList)
      };
    }

  }

}

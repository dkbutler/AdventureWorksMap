using ConversionServiceProject.POCOS.QueryData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace ConversionServiceProject.Services
{


  [ApiController]
  [Route("api/[controller]")]
  public class ConversionServiceController : ControllerBase

  {
        //This Service offers a bit more than mapquest geocoding service. It not only takes a list of addresses and returns their coordinates (which is all mapquest does),
        //it reattaches the names of the people and/or stores that were sent with each addresses. It also bypasses the limit of 100 addresses per call.
        //If ever a developer wanted to use mapquest geocoding API, but also wants results simliar to what I needed, they can call this service and save
        //themselves some time.

        //https://localhost:44320/Swagger 
        //https://addressconversionservice.azurewebsites.net/Swagger 


    [HttpPost("AddCoordinatesToAddresses")] //Only [HttpPost] if more than one post method in the controller. or else Leave off /AddCoordinatesToAddress in url
    public List<Location> AddCoordinatesToAddresses([FromBody] List<Location> addressList)
    {
      HttpClient client = new HttpClient();
      client.BaseAddress = new Uri("http://www.mapquestapi.com/");

      var addressDisplayList = new List<Location>();

      if (addressList?.Count > 0)
      {

        var batchesOf100locations = new List<List<Location>>();
        for (int i = 0; i < addressList.Count; i += 100)
        {
          batchesOf100locations.Add(addressList.GetRange(i, Math.Min(100, addressList.Count - i)));
        }

        var rootList = new List<Root>();
        for (int i = 0; i < batchesOf100locations.Count; i++)
        {
          Options options = new Options()
          {
            ignoreLatLngInput = false,
            maxResults = -1,
            thumbMaps = false
          };
          rootList.Add(new Root()
          {
            locations = batchesOf100locations[i],
            options = options
          });

        }

        foreach (var root in rootList)
        {

          var postResponse = client.PostAsJsonAsync("geocoding/v1/batch?key=Boe36yOAgjrKEZ0jtTZCCyAV7TNAfX4j", root);
          postResponse.Wait();
          var result = postResponse.Result;

          var readTask = result.Content.ReadAsAsync<POCOS.ResponseData.ResponseRoot>();
          readTask.Wait();
          var responseObj = readTask.Result;


          var i = 0;
          foreach (var responseResult in responseObj.results)
          {
            var coord = new Location();

            if (responseResult.locations.Count > 0)
            {
              coord.Lat = responseResult.locations[0].latLng.lat;
              coord.Long = responseResult.locations[0].latLng.lng;
              coord.city = responseResult.providedLocation.city;
              coord.state = responseResult.providedLocation.state;
              coord.street = responseResult.providedLocation.street;
              coord.personType = root.locations[i].personType;
              coord.name = root.locations[i].name;
              i++;
            }
            addressDisplayList.Add(coord);
          }

        }
      };

      return addressDisplayList;

    }

  }
}

using MapProject.Models;
using MapProject.POCOS.QueryData;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MapProject.Controllers.Interfaces
{
  public interface IMapController
  {
    MapViewModel GetCoordinates(List<Location> addressList);
    ActionResult GetMapPage(string filter = null);
    MapViewModel GetMap(string filter);
  }
}

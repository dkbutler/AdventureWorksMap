using MapProject.POCOS.QueryData;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MapProject.Services.Interfaces
{
  public interface IConversionServiceController
  {
    List<Location> AddCoordinatesToAddresses([FromBody] List<Location> addressList);
  }
}

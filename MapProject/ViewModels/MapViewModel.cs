using MapProject.AWData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapProject.Models
{
    public class MapViewModel
    { 
        public List<POCOS.QueryData.Location> Addresses { get; set; }
        public string Markers { get; set; }
        public int INcount { get; set; }
        public int EMcount { get; set; }
        public int SPcount { get; set; }
        public int STOREcount { get; set; }
        public int Allcount => INcount + EMcount + SPcount + STOREcount;

  }
}

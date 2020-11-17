using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapProject.POCOS.QueryData
{
    public class BatchRequest
    {
    }
    public class Location
    {
        public string name { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string street { get; set; }
        public string personType { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }

    }

    public class Options
    {
        public int maxResults { get; set; }
        public bool thumbMaps { get; set; }
        public bool ignoreLatLngInput { get; set; }
    }

    public class Root
    {
        public List<Location> locations { get; set; }
        public Options options { get; set; }
    }
}

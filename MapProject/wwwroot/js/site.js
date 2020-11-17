


function getMap(markers) {

  var map;
  var bounds = new google.maps.LatLngBounds();
  var mapOptions = {
    zoom: 2,
    center: { lat: 0, lng: 0 },
    mapTypeId: 'roadmap'
  };

  // Display a map on the page
  map = new google.maps.Map(document.getElementById("dvMap"), mapOptions);
  map.setTilt(45);

  // Loop through our array of markers & place each one on the map
  for (i = 0; i < markers.length; i++) {
    var position = new google.maps.LatLng(markers[i].Lat, markers[i].Long);
    bounds.extend(position);

    var color = "";
    if (markers[i].personType == "EM") { color = "https://maps.google.com/mapfiles/ms/icons/green-dot.png" }
    else if (markers[i].personType == "IN") { color = "https://maps.google.com/mapfiles/ms/icons/yellow-dot.png" }
    else if (markers[i].personType == "SP") { color = "https://maps.google.com/mapfiles/ms/icons/red-dot.png" }
    else { color = "https://maps.google.com/mapfiles/ms/icons/blue-dot.png" }

    marker = new google.maps.Marker({
      position: position,
      map: map,
      title: markers[i].name + ", " + markers[i].street,
      icon: {
        url: color
      }
    });
    // Automatically center the map fitting all markers on the screen
    map.fitBounds(bounds);
  }
}

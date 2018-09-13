//'use strict';


//function initMap() {

//    var map = new google.maps.Map(document.getElementById('map-with-marker'), {
//        zoom: 5,
//        scrollwheel: true,
//        center: { lat: 35.2289067, lng: -86.885620 }

//    });

//    var data = @Html.Raw(ViewBag.demos_map);

//    // Using the JQuery "each" selector to iterate through the JSON list and drop marker pins
//    $.each(data, function (i, item) {
//        var marker = new google.maps.Marker({
//            'position': new google.maps.LatLng(item.GeoLong, item.GeoLat),
//            'map': map,
//            'title': item.PlaceName
//        });


//        // Make the marker-pin blue!
//        if (item.demo_state == "On Hold") {
//            marker.setIcon('https://maps.google.com/mapfiles/ms/icons/orange-dot.png')
//        }
//        if (item.demo_state == "In progress") {
//            marker.setIcon('https://maps.google.com/mapfiles/ms/icons/green-dot.png')
//        }
//        if (item.demo_state == "Canceled") {
//            marker.setIcon('https://maps.google.com/mapfiles/ms/icons/red-dot.png')
//        }
//        if (item.demo_state == "Finished") {
//            marker.setIcon('https://maps.google.com/mapfiles/ms/icons/blue-dot.png')
//        }

//        // put in some information about each json object - in this case, the opening hours.

//        //colocamos la url de la demo
//        var url = '@Url.Action("PreviewSendDemoResume", "Demos", new { id = "DEMOID"})';


//        url = url.replace('DEMOID', item.id);

//        var infowindow = new google.maps.InfoWindow({
//            content: "<div class='infoDiv'><h8>Place: " + item.PlaceName + ".</h8><br>" + "<div><h8>Vendor: " + item.vendor + ".</h8><br><h8>Visit date: " + item.date + ".</h8><br><h8>User: " + item.nombre + ".</h8><br><h8>Comment: " + item.comment + ".</h8><br><a href='" + url + "' class='btn btn-primary' target='_blank'>Preview</a></div></div>"
//        });

//        // finally hook up an "OnClick" listener to the map so it pops up out info-window when the marker-pin is clicked!
//        google.maps.event.addListener(marker, 'click', function () {
//            infowindow.open(map, marker);
//        });

//        marker.addListener('dblclick', function () {
//            map.setZoom(18);
//            map.setCenter(marker.getPosition());

//            // We get the map's default panorama and set up some defaults.
//            // Note that we don't yet set it visible.
//            panorama = map.getStreetView();
//            panorama.setPosition(marker.getPosition());
//            panorama.setPov(({
//                heading: 265,
//                pitch: 0
//            }));
//        });



//    })
//}

//    var markers = locations.map(function (location, i) {
//        return new google.maps.Marker({
//            position: location,
//            label: labels[i % labels.length]
//        });
//    });

//    // Add a marker clusterer to manage the markers.
//    var markerCluster = new MarkerClusterer(map, markers,
//        { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });


//    for (var i = 0; i < markers.length; i++) {
    
//        google.maps.event.addListener(markers[i], 'click', function () {
//            //alert(this.position + " - " + this.label);

//            myFunction(this.position, this.label);

//        });
//    }
//}
//var locations = [
//    { lat: 35.563910, lng: -85.785620 },
//    { lat: 36.718234, lng: -84.785620 },
//    { lat: 35.727111, lng: -83.785620 },
//    { lat: 36.848588, lng: -82.785620 },
//    { lat: 35.851702, lng: -81.785620 },
//    { lat: 37.671264, lng: -86.185620 },
//    { lat: 35.563910, lng: -85.785620 },
//    { lat: 35.718234, lng: -84.785620 },
//    { lat: 35.727111, lng: -83.785620 },
//    { lat: 35.848588, lng: -82.785620 },
//    { lat: 35.851702, lng: -81.785620 },
//    { lat: 35.671264, lng: -86.185620 },
//    { lat: 35.851702, lng: -81.785620 },
//    { lat: 35.671264, lng: -86.185620 },
//    { lat: 36.999792, lng: -85.185620 }
//]

//function myFunction(position, label) {
//    var x = document.getElementById("RouteDetails");
//    if (x.style.display === "none") {
//        x.style.display = "block";
//        var id_mapitem = document.getElementById("map_id");
//        id_mapitem.innerHTML = "ID: " + label;
//    } else {
//        x.style.display = "none";
//    }
//}

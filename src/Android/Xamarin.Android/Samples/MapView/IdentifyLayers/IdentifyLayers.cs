// Copyright 2018 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Android.App;
using Android.OS;
using Android.Widget;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;

namespace ArcGISRuntimeXamarin.Samples.IdentifyLayers
{
    [Activity]
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Identify layers",
        "MapView",
        "This sample demonstrates how to identify features from multiple layers in a map.",
        "")]
    public class IdentifyLayers : Activity
    {
        // Create and hold reference to the used MapView.
        private readonly MapView _myMapView = new MapView();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Title = "Identify layers";

            CreateLayout();
            Initialize();
        }

        private async void Initialize()
        {
            // Create a map with an initial viewpoint.
            Map myMap = new Map(Basemap.CreateTopographic());
            myMap.InitialViewpoint = new Viewpoint(new MapPoint(-10977012.785807, 4514257.550369, SpatialReference.Create(3857)), 68015210);
            _myMapView.Map = myMap;

            try
            {
                // Add a map image layer to the map after turning off two sublayers.
                ArcGISMapImageLayer cityLayer = new ArcGISMapImageLayer(new Uri("https://sampleserver6.arcgisonline.com/arcgis/rest/services/SampleWorldCities/MapServer"));
                await cityLayer.LoadAsync();
                cityLayer.Sublayers[1].IsVisible = false;
                cityLayer.Sublayers[2].IsVisible = false;
                myMap.OperationalLayers.Add(cityLayer);

                // Add a feature layer to the map.
                FeatureLayer damageLayer = new FeatureLayer(new Uri("https://sampleserver6.arcgisonline.com/arcgis/rest/services/DamageAssessment/FeatureServer/0"));
                myMap.OperationalLayers.Add(damageLayer);

                // Listen for taps/clicks to start the identify operation.
                _myMapView.GeoViewTapped += MyMapView_GeoViewTapped;
            }
            catch (Exception e)
            {
                new AlertDialog.Builder(this).SetMessage(e.ToString()).SetTitle("Error").Show();
            }
        }

        private async void MyMapView_GeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            try
            {
                // Perform an identify across all layers, taking up to 10 results per layer.
                IReadOnlyList<IdentifyLayerResult> identifyResults = await _myMapView.IdentifyLayersAsync(e.Position, 15, false, 10);

                // Add a line to the output for each layer, with a count of features in the layer.
                string result = "";
                foreach (IdentifyLayerResult layerResult in identifyResults)
                {
                    // Note: because some layers have sublayers, a recursive function is required to count results.
                    result = result + layerResult.LayerContent.Name + ": " + recursivelyCountIdentifyResultsForSublayers(layerResult) + "\n";
                }

                if (!String.IsNullOrEmpty(result))
                {
                    new AlertDialog.Builder(this).SetMessage(result).SetTitle("Identify result").Show();
                }
            }
            catch (Exception ex)
            {
                new AlertDialog.Builder(this).SetMessage(ex.ToString()).SetTitle("Error").Show();
            }
        }

        private int recursivelyCountIdentifyResultsForSublayers(IdentifyLayerResult result)
        {
            int sublayerResultCount = 0;
            foreach (IdentifyLayerResult res in result.SublayerResults)
            {
                // This function calls itself to count results on sublayers.
                sublayerResultCount += recursivelyCountIdentifyResultsForSublayers(res);
            }

            return result.GeoElements.Count + sublayerResultCount;
        }

        private void CreateLayout()
        {
            // Create a new vertical layout for the app.
            var layout = new LinearLayout(this) {Orientation = Orientation.Vertical};

            // Create and add a help tip.
            TextView helpLabel = new TextView(this)
            {
                Text = "Tap to identify features in all layers."
            };
            layout.AddView(helpLabel);

            // Add the map view to the layout.
            layout.AddView(_myMapView);

            // Show the layout in the app.
            SetContentView(layout);
        }
    }
}
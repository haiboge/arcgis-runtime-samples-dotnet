﻿// Copyright 2019 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Portal;
using Xamarin.Forms;

namespace ArcGISRuntimeXamarin.Samples.ViewContentBeneathSurface
{
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "View content beneath terrain surface",
        "Map",
        "See through terrain in a scene and move the camera underground.",
        "")]
    public partial class ViewContentBeneathSurface : ContentPage
    {
        public ViewContentBeneathSurface()
        {
            InitializeComponent();
            Initialize();
        }

        private async void Initialize()
        {
            // Load the item from ArcGIS Online.
            PortalItem webSceneItem = await PortalItem.CreateAsync(await ArcGISPortal.CreateAsync(), "91a4fafd747a47c7bab7797066cb9272");

            // Load the web scene from the item.
            Scene webScene = new Scene(webSceneItem);

            // Show the web scene in the view.
            MySceneView.Scene = webScene;
        }
    }
}

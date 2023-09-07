﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace PipedEverything
{
    [SkipSaveFileSerialization]
    internal class PortDisplayController : KMonoBehaviour
    {
        [SerializeField]
        private HashedString lastMode = OverlayModes.None.ID;

        [SerializeField]
        private List<PortDisplay2> gasOverlay = new();

        [SerializeField]
        private List<PortDisplay2> liquidOverlay = new();

        [SerializeField]
        private List<PortDisplay2> solidOverlay = new();

        public void AssignPort(GameObject go, DisplayConduitPortInfo port)
        {
            PortDisplay2 portDisplay = go.AddComponent<PortDisplay2>();
            portDisplay.AssignPort(port);

            switch (port.type)
            {
                case ConduitType.Gas:
                    this.gasOverlay.Add(portDisplay);
                    break;
                case ConduitType.Liquid:
                    this.liquidOverlay.Add(portDisplay);
                    break;
                case ConduitType.Solid:
                    this.solidOverlay.Add(portDisplay);
                    break;
            }
        }

        public void Init(GameObject go)
        {
            string ID = go.GetComponent<KPrefabID>().PrefabTag.Name;

            // criteria for drawing port icons on buildings
            // vanilla will only attempt to draw icons on buildings with BuildingCellVisualizer
            go.AddOrGet<BuildingCellVisualizer>();

            // when vanilla tries to draw, call this controller if the building is in the DrawPorts list
            Patches.DrawBuildings.Add(ID);
        }

        public bool Draw(BuildingCellVisualizer __instance, HashedString mode, GameObject go)
        {
            bool isNewMode = mode != this.lastMode;

            if (isNewMode)
            {
                ClearPorts();
                this.lastMode = mode;
            }

            foreach (PortDisplay2 port in GetPorts(mode))
            {
                port.Draw(go, __instance, isNewMode);
            }

            return true;
        }

        private void ClearPorts()
        {
            foreach (PortDisplay2 port in GetPorts(this.lastMode))
            {
                port.DisableIcons();
            }
        }

        private List<PortDisplay2> GetPorts(HashedString mode)
        {
            if (mode == OverlayModes.GasConduits.ID) return this.gasOverlay;
            if (mode == OverlayModes.LiquidConduits.ID) return this.liquidOverlay;
            if (mode == OverlayModes.SolidConveyor.ID) return this.solidOverlay;

            return new List<PortDisplay2>();
        }

        public bool IsInputConnected(Element element)
        {
            var hash = element.id;
            foreach (var port in element.IsGas ? gasOverlay : element.IsLiquid ? liquidOverlay : solidOverlay)
            {
                if (port.input && port.filter.Contains(hash))
                    return port.IsConnected();
            }
            return false;
        }

        public bool IsOutputConnected(Element element)
        {
            var hash = element.id;
            foreach (var port in element.IsGas ? gasOverlay : element.IsLiquid ? liquidOverlay : solidOverlay)
            {
                if (!port.input && port.filter.Contains(hash))
                    return port.IsConnected();
            }
            return false;
        }
    }
}

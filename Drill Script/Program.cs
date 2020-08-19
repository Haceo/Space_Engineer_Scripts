﻿using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;
using System.Collections.Specialized;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        double percent = 0;
        Color fontColor;
        IMyTextSurface _drawingSurfaceCapacity;
        RectangleF _viewportCapacity;
        string[] drillBlocks;
        public Program()
        {
            if (Me.CustomData.Length < 4)
            {
                Echo("Error, Input custom data!");
                return;
            }
            drillBlocks = Me.CustomData.Split('\n');
            _drawingSurfaceCapacity = (GridTerminalSystem.GetBlockWithName(drillBlocks[0]) as IMyCockpit).GetSurface(0);
            _drawingSurfaceCapacity.ContentType = ContentType.SCRIPT;
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            _viewportCapacity = new RectangleF(
                (_drawingSurfaceCapacity.TextureSize - _drawingSurfaceCapacity.SurfaceSize) / 2f,
                _drawingSurfaceCapacity.SurfaceSize);
        }

        public void Main(string argument, UpdateType updateSource)
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            foreach (var name in drillBlocks)
                blocks.Add(GridTerminalSystem.GetBlockWithName(name));
            blocks.RemoveAt(0);
            if (blocks.Count < 1)
            {
                Echo("Error, No blocks found!");
                return;
            }
            float vol = 0f;
            float maxVol = 0f;
            foreach (var block in blocks)
            {
                vol += block.GetInventory().CurrentVolume.RawValue;
                maxVol += block.GetInventory().MaxVolume.RawValue;
            }
            percent = Math.Round((vol / maxVol) * 100, 2);
            Echo($"{blocks.Count} Cargo Blocks");
            var frame = _drawingSurfaceCapacity.DrawFrame();
            if (percent < 50)
                fontColor = Color.Green;
            else if (percent > 50)
                fontColor = Color.Yellow;
            else if (percent > 75)
                fontColor = Color.Red;
            DrawSprites(ref frame);
            frame.Dispose();
        }

        public void DrawSprites(ref MySpriteDrawFrame frame)
        {
            var pos = new Vector2(256, 124) + _viewportCapacity.Position;
            var sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = $"{percent}%",
                Position = pos,
                RotationOrScale = 3f,
                Color = fontColor,
                Alignment = TextAlignment.CENTER,
                FontId = "White"
            };
            frame.Add(sprite);
        }
    }
}

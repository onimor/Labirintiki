﻿using LabirintBlazorApp.Common;
using LabirintBlazorApp.Dto;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components;

public partial class MazeSands : CanvasComponent
{
    [Parameter]
    [EditorRequired]
    public required int[,] Sand { get; set; }

    [Parameter]
    [EditorRequired]
    public required int HalfBoxSize { get; set; }

    [Parameter]
    [EditorRequired]
    public required int WallWidth { get; set; }

    [Parameter]
    [EditorRequired]
    public required Vision Vision { get; set; }

    private int MazeWidth => Sand.GetLength(0);
    private int MazeHeight => Sand.GetLength(1);

    protected override int CanvasWidth => MazeWidth * HalfBoxSize;
    protected override int CanvasHeight => MazeHeight * HalfBoxSize;

    protected override string CanvasId => "mazeSandsCanvas";

    protected override async Task DrawAsync()
    {
        await Context.ClearRectAsync(0, 0, CanvasWidth, CanvasHeight);
        await Context.SetStrokeStyleAsync(Parameters.Labyrinth.Color);
        await Context.SetLineWidthAsync(WallWidth);

        List<DrawCommand> drawCommands = [];

        int entityBoxSize = HalfBoxSize * 2 - WallWidth;
        int offset = HalfBoxSize - entityBoxSize;

        for (int y = Vision.StartY; y < Vision.FinishY; y += 2)
        {
            for (int x = Vision.StartX; x < Vision.FinishX; x += 2)
            {
                if (Sand[y, x] != 0)
                {
                    continue;
                }

                var drawY = Vision.GetDrawY(y);
                var drawX = Vision.GetDrawX(x);

                int left = (drawX - 1) * HalfBoxSize + WallWidth - offset / 2;
                int top = (drawY - 1) * HalfBoxSize + WallWidth - offset;

                drawCommands.Add(new DrawCommand(DrawCommandType.DrawImage, "/images/sand.png", left, top, HalfBoxSize));
            }
        }

        Logger.LogInformation("Количество команд на отрисовку (sand): {Count}", drawCommands.Count);

        await Context.DrawCommandsAsync(drawCommands);
    }

    public async Task UpdateAsync(int x, int y)
    {
        int left = (x - 1) * HalfBoxSize + WallWidth;
        int top = (y - 1) * HalfBoxSize + WallWidth;
        int entityBoxSize = HalfBoxSize * 2 - WallWidth;

        if (IsDebug)
        {
            await Context.FillRectAsync(left, top, entityBoxSize, entityBoxSize);
        }
        else
        {
            await Context.ClearRectAsync(left, top, entityBoxSize, entityBoxSize);
        }
    }
}

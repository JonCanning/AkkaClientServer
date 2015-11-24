﻿open System.Drawing
open System.IO
open System


type Plotter = {
    position    : int*int
    color       : Color
    direction   : float
    bitmap      : Bitmap
    }

type TurtleCmd =
    | Turn of float
    | Move of int32
    | Polygon of int32*int32

let naiveLine (x1,y1) plotter =
    let updatedPlotter = { plotter with position=(x1,y1) }
    let (x0,y0) = plotter.position
    let xLen = float (x1-x0)
    let yLen = float (y1-y0)

    let x0,y0,x1,y1 = if x0 > x1 then x1,y1,x0,y0 else x0,y0,x1,y1
    if xLen <> 0.0 then
        for x in x0..x1 do
            let proportion = float (x-x0) / xLen
            let y = int (Math.Round(proportion * yLen)) + y0
            plotter.bitmap.SetPixel(x,y, plotter.color)

    let x0,y0,x1,y1 = if y0 > y1 then x1,y1,x0,y0 else x0,y0,x1,y1
    if yLen <> 0.0 then
        for y in y0..y1 do
            let proportion = float (y-y0) / yLen
            let x = int (Math.Round(proportion * xLen)) + x0
            plotter.bitmap.SetPixel(x,y, plotter.color)

    updatedPlotter

let turn amt plotter =
    let newDir = plotter.direction + amt
    let angled = { plotter with direction=newDir }
    printfn "%A" angled
    angled

let move dist plotter =
    let currPos = plotter.position
    let angle   = plotter.direction
    let startX  = fst currPos
    let startY  = snd currPos
    let rads    = (angle - 90.0) * Math.PI/180.0
    let roundX  = (float startX) + (float dist) * cos rads
    let roundY  = (float startY) + (float dist) * sin rads
    let endX    = int (Math.Round(roundX))
    let endY    = int (Math.Round(roundY))
    let plotted = naiveLine (endX, endY) plotter
    printfn "%A" plotted
    plotted

let polygon (sides:int) length plotter =
    let angle = Math.Round(360.0/float sides)
    Seq.fold (fun s i -> turn angle (move length s)) plotter [1.0..(float sides)]

let semiCirc (sides:int) length plotter =
    let angle = Math.Round(360.0/float sides)
    Seq.fold (fun s i -> turn angle (move length s)) plotter [1.0..(float sides/2.0)]

let thirdCirc (sides:int) length plotter =
    let angle = Math.Round(360.0/float sides)
    Seq.fold (fun s i -> turn angle (move length s)) plotter [1.0..(float sides/3.0)]

let fifthCirc (sides:int) length plotter =
    let angle = Math.Round(360.0/float sides)
    Seq.fold (fun s i -> turn angle (move length s)) plotter [1.0..(float sides/5.0)]

let fifthteenth (sides:int) length plotter =
    let angle = Math.Round(360.0/float sides)
    Seq.fold (fun s i -> turn angle (move length s)) plotter [1.0..(float sides/15.0)]

let moveTo (x1,y1) plotter =
    { plotter with position=(x1,y1) }

let changeColor color plotter =
    { plotter with color=color }

let saveAs name plotter =
    let sequencePath = Path.Combine(__SOURCE_DIRECTORY__, name)
    plotter.bitmap.Save(sequencePath)

let generate cmdStripe times fromPlotter =
    let cmdsGen = 
        seq { 
            while true do 
                yield! cmdStripe }

    let cmds = cmdsGen |> Seq.take (times*(cmdStripe |> List.length))

    cmds |> Seq.fold (fun plot cmd -> cmd plot) fromPlotter 

let turtleCommandInterpreter (turtleCmd:TurtleCmd) =
    match turtleCommand with
    | Move (dist) ->
        move dist
    | Turn (angle) ->
        turn angle
    | Polygon (sides,length) ->
        polygon sides length
using System;
using SheetMusicToPianoKeys;

Console.WriteLine("Sheet music to piano keys converter");

var parser = new MxlParser();

Console.Write("Piano or keyboard? (middle C is C3 or C4?): ");

string input = Console.ReadLine();
input = input.ToLower();

if (input.Contains("4") || input.Contains("piano") || input.Contains("no"))
{
    parser.IsKeyboard = false;
}
else
{
    parser.IsKeyboard = true;
}

Console.Write("Paste path to .mxl file: ");

string path = Console.ReadLine();

var score = new MxlConverter().Convert(path);

var (right, left) = new MxlParser().Parse(score);

Console.WriteLine("Right hand:");
Console.WriteLine(right);

Console.WriteLine("------------------");

Console.WriteLine("Left hand:");
Console.WriteLine(left);
﻿using System;
using System.Diagnostics;
using Edelstein.Common.Gamplay.Stages.Game.Templates;
using Edelstein.Common.Parser.Duey;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var loggerFactory = new LoggerFactory().AddSerilog();
            var collection = new NXDataDirectoryCollection(args[0]);
            var fieldTemplates = new FieldTemplateRepository(
                collection,
                loggerFactory.CreateLogger<FieldTemplateRepository>()
            );
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var fieldTemplate = fieldTemplates.Retrieve(310000000).Result;

            Console.WriteLine($"Edelstein is {fieldTemplate.Bounds.Size.Width}x{fieldTemplate.Bounds.Size.Height} big");
            Console.WriteLine($"Took {stopwatch.Elapsed} to retrieve");
        }
    }
}

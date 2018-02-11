﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WoWFormatLib;
using WoWFormatLib.DBC;
using WoWFormatLib.FileReaders;
using WoWFormatLib.Utils;

namespace MinimapCompiler
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            string basedir = ConfigurationManager.AppSettings["basedir"];
            bool buildmaps = bool.Parse(ConfigurationManager.AppSettings["buildmaps"]);
            bool buildWMOmaps = bool.Parse(ConfigurationManager.AppSettings["buildwmomaps"]);

            Console.WriteLine("Initializing CASC..");

            if(basedir != string.Empty){
                CASC.InitCasc(null, basedir);
            }else{
                CASC.InitCasc(null, null, "wowt");
            }

            Console.WriteLine("CASC initialized!");
            Console.WriteLine("Current patch: " + CASC.cascHandler.Config.BuildName);

            /*
            string mapname = "";
             * 
            if (buildmaps == true)
            {
                DB2Reader<MapRecordLegion> reader = new DB2Reader<MapRecordLegion>("DBFilesClient\\Map.db2");
                for (int i = 0; i < reader.recordCount; i++)
                {
                    //I used to check if WDT existed, but sometimes minimaps for maps without WDTs slip through the cracks
                    mapname = reader[i].Directory;

                   // if (reader[i].expansionID < 6) { Console.WriteLine("Skipping map " + mapname + " WoD and below!"); continue; }

                    var min_x = 64;
                    var min_y = 64;

                    var max_x = 0;
                    var max_y = 0;

                    for (int cur_x = 0; cur_x < 64; cur_x++)
                    {
                        for (int cur_y = 0; cur_y < 64; cur_y++)
                        {
                            if (CASC.FileExists("World\\Minimaps\\" + mapname + "\\map" + cur_x + "_" + cur_y + ".blp"))
                            {
                                if (cur_x > max_x) { max_x = cur_x; }
                                if (cur_y > max_y) { max_y = cur_y; }

                                if (cur_x < min_x) { min_x = cur_x; }
                                if (cur_y < min_y) { min_y = cur_y; }
                            }
                        }
                    }

                    Console.WriteLine("[" + mapname + "] MIN: (" + min_x + " " + min_y + ") MAX: (" + max_x + " " + max_y + ")");

                    var res_x = (((max_x - min_x) * 256) + 256);
                    var res_y = (((max_y - min_y) * 256) + 256);

                    if (res_x < 0 || res_y < 0)
                    {
                        Console.WriteLine("[" + mapname + "] " + "Skipping map, has no minimap tiles");
                        continue;
                    }

                    Console.WriteLine("[" + mapname + "] " + "Creating new image of " + res_x + "x" + res_y);


                    Bitmap bmp = new Bitmap(res_x, res_y);
                    Graphics g = Graphics.FromImage(bmp);

                    Font drawFont = new Font("Arial", 16);

                    for (int cur_x = 0; cur_x < 64; cur_x++)
                    {
                        for (int cur_y = 0; cur_y < 64; cur_y++)
                        {
                            if (CASC.FileExists("World\\Minimaps\\" + mapname + "\\map" + cur_x + "_" + cur_y + ".blp"))
                            {
                                var blpreader = new BLPReader();
                                blpreader.LoadBLP("World\\Minimaps\\" + mapname + "\\map" + cur_x + "_" + cur_y + ".blp");
                                g.DrawImage(blpreader.bmp, (cur_x - min_x) * 256, (cur_y - min_y) * 256, new Rectangle(0, 0, 256, 256), GraphicsUnit.Pixel);
                            }
                        }
                    }
                    g.Dispose();
                    if (!Directory.Exists("done")) { Directory.CreateDirectory("done"); }
                    bmp.Save("done/" + mapname + ".png");
                    
                    // SUPER MINIMAP COMPILER TIME!!!!!!!!!!!!!!!
                    /*
                    var super_res_x = (((max_x - min_x) * 512) + 512);
                    var super_res_y = (((max_y - min_y) * 512) + 512);

                    if (super_res_x < 0 || super_res_y < 0)
                    {
                        Console.WriteLine("[SUPER " + mapname + "] " + "Skipping map, has no minimap tiles");
                        continue;
                    }

                    Console.WriteLine("[SUPER " + mapname + "] " + "Creating new image of " + super_res_x + "x" + super_res_y);

                    Bitmap super_bmp = new Bitmap(super_res_x, super_res_y);
                    Graphics super_g = Graphics.FromImage(super_bmp);

                    for (int cur_x = 0; cur_x < 64; cur_x++)
                    {
                        for (int cur_y = 0; cur_y < 64; cur_y++)
                        {
                            if (CASC.FileExists("World\\Minimaps\\" + mapname + "\\map" + cur_x + "_" + cur_y + ".blp"))
                            {
                                var blpreader = new BLPReader();
                                blpreader.LoadBLP("World\\Minimaps\\" + mapname + "\\map" + cur_x + "_" + cur_y + ".blp");
                                super_g.DrawImage(blpreader.bmp, (cur_x - min_x) * 512, (cur_y - min_y) * 512, new Rectangle(0, 0, 512, 512), GraphicsUnit.Pixel);
                            }
                        }
                    }
                    super_g.Dispose();
                    if (!Directory.Exists("done")) { Directory.CreateDirectory("done"); }
                    super_bmp.Save("done/SUPER_" + mapname + ".png");
                    */
            // }
            //}


            if (buildWMOmaps == true)
            {
                List<string> linelist = new List<string>();
                
                foreach(var line in File.ReadAllLines("listfile.txt"))
                {
                     linelist.Add(line);
                }

                string[] unwantedExtensions = new string[513];
                for (int i = 0; i < 512; i++)
                {
                    unwantedExtensions[i] = "_" + i.ToString().PadLeft(3, '0') + ".wmo";
                }

                unwantedExtensions[512] = "lod1.wmo";
                foreach (string s in linelist)
                {
                    if (s.Length > 8 && !unwantedExtensions.Contains(s.Substring(s.Length - 8, 8)))
                    {
                        if ((!s.Contains("lod1.wmo") && !s.Contains("lod2.wmo")) && s.EndsWith(".wmo", StringComparison.CurrentCultureIgnoreCase))
                        {
                            Console.WriteLine(s);
                            WMO wmocompiler = new WMO();
                            wmocompiler.Compile(s);
                        }
                    }
                }
                Console.ReadLine();
            }
        }
    }
}

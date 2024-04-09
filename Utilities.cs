namespace A3_Modpack_Parser;

using HtmlAgilityPack;
using System;

public class Utilities{
    
    // Take in file path to an HTML file, parse it and return a dictionary<string,string> with mod names as keys, workshop url's as values 
    public Dictionary<string,string> GetModList(string htmlContent){
        HtmlDocument doc = new HtmlDocument();
        Console.WriteLine("Attempting to parse modlist from file: " + htmlContent);
        doc.Load(htmlContent);

        HtmlNodeCollection nodes = null;

        try{
            nodes = doc.DocumentNode
            .SelectNodes("//body/div[@class='mod-list']/table/tr[@data-type='ModContainer']");
        }catch (Exception e){
            Console.WriteLine("Unknown HTML file detected, skipping file.\nFull error: " + e);
            return null;
        }

        Dictionary<string,string> modDict = new Dictionary<string,string>();
        int counter = 1;
        foreach (HtmlNode i in nodes)
        {
            // By reading this comment, you release me from responsibility should you lose braincells reading the next few lines
            if(i.SelectSingleNode("//body/div[@class='mod-list']/table/tr[@data-type='ModContainer'][" + counter + "]/td[2]/span").InnerHtml == "Steam"){
                string keyNodeToLookFor = "//body/div[@class='mod-list']/table/tr[@data-type='ModContainer'][" + counter + "]/td[1]";
                string valueNodeToLookFor = "//body/div[@class='mod-list']/table/tr[@data-type='ModContainer'][" + counter + "]/td[3]/a";
                
                string key = i.SelectSingleNode(keyNodeToLookFor).InnerHtml;
                string value = i.SelectSingleNode(valueNodeToLookFor).InnerHtml;
                
                try{
                    modDict.Add(key,value);
                }catch{
                    Console.WriteLine("failed to add: " + key + " " + value);
                }
            }
            counter++;
        }
        Console.WriteLine("Successfully parsed file: " + htmlContent);
        return modDict;
    }


    // Request a folder path containing preset files, process each and output a dictionary<string,string> of mods used by the given presets
    public Dictionary<string,string> usedModsDict = new Dictionary<string,string>();
    public bool AddPresetsToList(){
        
        //Check if master preset has been added
        if(masterModsDict.Count == 0){
            Console.WriteLine("You must add a master list first!");
            Console.WriteLine("Press Enter to return to the menu...");
            Console.ReadLine();
            return false;
        }

        Console.WriteLine("\nPlease add all preset files currently in use to the 'Presets' folder inside this programs folder.");
        Console.WriteLine(@"
Quick guide:
        1. Gather up all preset files in use by all communities you play in.
        2. Copy them to the folder 'A3_Modpack_Parser/Presets/'
        
NOTE: ENSURE THERE ARE NO OTHER HTML FILES PRESENT IN THE SAME FOLDER, EXCEPT FOR ARMA PRESETS YOU WISH TO INPUT
        ");
        Console.Write("Press Enter to continue...");
        Console.ReadLine();
        try{
            string[] fileList = Directory.GetFiles(Path.GetFullPath("Presets"),"*.html");

            foreach (var file in fileList)
            {
                try{
                    var tempDict = GetModList(file);
                    foreach (var i in tempDict)
                    {
                        try{
                            usedModsDict.Add(i.Key,i.Value);
                            Console.WriteLine("Added " + i + " to list");
                        }catch{
                            Console.WriteLine("Mod '" + i.Key + "' already seen, skipping...");
                        }
                    }
                }catch(Exception e){
                    Console.WriteLine("Bad HTML file detected. Full error: " + e); // I have no idea if this even works, should one of the HTML files not be a preset
                    return false;
                }
                
            }
            Console.WriteLine("Successfully parsed all files.");
            return true;
        }catch (Exception e){
            Console.WriteLine("Error: " + e);
            return false;
        }
    }

    // Request a preset file containing all subscribed mods of the user, and create a dictionary<string,string> from it to compare against.
    public Dictionary<string,string> masterModsDict = new Dictionary<string,string>();
    public bool CreateMasterList(){
        
        Console.WriteLine("\nPlease save a preset file named 'master' that includes all mods you have subscribed to, to the 'FullMods' folder in this programs folder.");
        Console.WriteLine(@"
Quick guide:
        1. Launch the Arma 3 Launcher and go into the 'Mods' tab.
        2. Press the 'More' button above the list.
        3. Select 'Export a list of mods to a file', and in the next window press 'All mods'.
        4. Name the file as 'master.html'.
        5. Save or move the file into the 'A3_Modpack_Parser/FullMods' folder.
        ");
        Console.Write("Press Enter to continue...");
        Console.ReadLine();

        masterModsDict.Clear(); // Empty out any previous items, just to keep things simpler in case user does the process multiple times.

        try{
            var tempDict = GetModList(Path.GetFullPath("FullMods\\master.html"));
            foreach (var i in tempDict)
            {
                try{
                    masterModsDict.Add(i.Key,i.Value);
                    Console.WriteLine("Added " + i + " to list");
                }catch{
                    Console.WriteLine("Mod '" + i.Key + "' already seen, skipping...");
                }
            }
            Console.WriteLine("Master list successfully added.");
            Console.WriteLine("Press Enter to return to the menu.");
            Console.ReadLine();
            return true;
        }catch(Exception e){
            Console.WriteLine("Bad HTML file detected. Full error: " + e);
            return false;
        }
    }

    public void FindUnusedMods(){
        foreach (var master in masterModsDict)
        {
            foreach (var used in usedModsDict)
            {
                string value = "";
                if(usedModsDict.TryGetValue(master.Key, out value)){
                    masterModsDict.Remove(used.Key);
                }
            }
        }
    }

    public void ComposeHTML(){
        Console.WriteLine("Creating HTML file...");
        string htmlContent = "";
        foreach (var i in masterModsDict)
        {
            htmlContent = htmlContent + ContentStrings.ModifyToModlist(i.Key,i.Value);
            Console.WriteLine("Progress: " + i);
        }
        htmlContent =  ContentStrings.htmlBefore + htmlContent + ContentStrings.htmlAfter;
        StreamWriter sw = new(Path.GetFullPath("unsubscribable.html"));
        // foreach (var i in htmlContent)
        // {
        //     sw.WriteLine(htmlContent);
        // }
        sw.WriteLine(htmlContent);
        sw.Close();

        Console.WriteLine("Preset file 'unsubscribable.html' was created in the program folder. Import it to the A3 launcher and you can unsubscribe all mods active in it.");
        Console.Write("Press Enter to return to the main menu.");
        Console.ReadLine();

    }

    // Singleton Pattern
    private static Utilities _instance;
    private static readonly object _lock = new object();
    public static Utilities GetInstance()
    {
        if(_instance == null)
        {
            lock(_lock)
            {
                if(_instance == null)
                {
                    _instance = new Utilities();
                }
            }
        }
        return _instance;
    }
}
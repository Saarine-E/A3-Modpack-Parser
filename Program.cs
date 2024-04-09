namespace A3_Modpack_Parser;

using System;

public class Program
{
	public static void Main(){
            
        Utilities utils = Utilities.GetInstance();

        utils.CreateMasterList();
        utils.AddPresetsToList();
        utils.FindUnusedMods();
        utils.ComposeHTML();

        Console.WriteLine("File 'unsubscribe.html' was successfully created.");
        Console.Write("Press enter to exit program...");
        Console.ReadLine();
        Environment.Exit(0);
    }
}
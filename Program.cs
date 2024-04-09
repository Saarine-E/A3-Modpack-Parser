namespace A3_Modpack_Parser;

using System;
using System.Security.Cryptography.X509Certificates;

public class Program
{
	public static void Main(){
            
        Utilities utils = Utilities.GetInstance();

        Console.WriteLine(@"
*******************************************************
           Welcome to the A3 Modpack Parser
    ***********************************************
           Made by Saarine-E aka Sienihemmo
*******************************************************
    Please select an option:
        1. Add a master list of all subscribed mods (REQUIRED).
        2. Create a preset of unused A3 mods that can be unsubscribed from workshop.
        3. Exit the program (You can also just close the window)
        ");
        Console.Write("Input(1-3): ");

        switch(Convert.ToInt32(Console.ReadLine())){

            case 1:
            utils.CreateMasterList();
            break;

            case 2:
            if (utils.AddPresetsToList()){
                utils.FindUnusedMods();
                utils.ComposeHTML();
            };
            break;

            case 3:
            Environment.Exit(0);
            break;

            default:
            break;
        }
        
        Main();
    }
}
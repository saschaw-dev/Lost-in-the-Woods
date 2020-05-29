using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; //Beinhaltet Editor-Programmierung

public class CreateInventoryItem{ //darf nicht von MonoBehaviour erben, weil es nicht an Szenenobjekt gehängt werden soll und die Methoden aus der Klasse Monobehaviour nicht benötigt werden

    [MenuItem("Assets/Create/Inventory Item")] //wo soll der Menuebutton erstellt werden und wie soll er heissen?
    static void CreateAsset() //Methode die ausgeführt wird, wenn Button geklickt wird
    {
        //Gibt es einen Unterordner "Inventory Items" im Project Browser?
        if(!AssetDatabase.IsValidFolder("Assets/Scripts/Inventory Items"))
        {
            //Wenn nicht, dann erstellen wir diesen
            AssetDatabase.CreateFolder("Assets/Scripts", "Inventory Items");

        }
        //Neue Instanz von InventoryItem erstellen
        ScriptableObject asset = ScriptableObject.CreateInstance(typeof(InventoryItem));
        //Aus der erstellten Instanz ein Asset im Project Browser erstellen
        AssetDatabase.CreateAsset(asset, "Assets/Scripts/Inventory Items/" + "New InventoryItem" + System.Guid.NewGuid() + ".asset");
        //Alle ungesicherten Asset-Aenderungen spreichern
        AssetDatabase.SaveAssets();
        //Alle  Aenderungen neuladen
        AssetDatabase.Refresh();
        //Den Fokus auf den Project Browser legen
        EditorUtility.FocusProjectWindow();
        //Das neue Asset im Project Browser selektieren
        Selection.activeObject = asset;
    }
	
}

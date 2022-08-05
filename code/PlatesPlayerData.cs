using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public partial class PlatesPlayerData
{
    public int Money {get;set;} = 0;

    private static PlatesPlayerData Local = null;
    public static PlatesPlayerData GetLocalData()
    {
        if(Local == null) Local = Get();
        return Local;
    }

    public void Save()
    {
        FileSystem.Data.WriteJson("playerdata.json", this);
    }

    public static PlatesPlayerData Get()
    {
        return FileSystem.Data.ReadJsonOrDefault<PlatesPlayerData>("playerdata.json", new());
    }


    [ClientRpc]
    public static void GiveMoney(int value)
    {
        var data = GetLocalData();
        data.Money += value;
        data.Save();
    }

}
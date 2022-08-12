using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public static partial class PlayerDataManager
{
    [Net] private static IDictionary<long, PlayerData> Players {get;set;} = new Dictionary<long, PlayerData>();
    public static PlayerData LocalData
    {
        get
        {
            if(Players.Keys.Contains(Local.PlayerId)) return Players[Local.PlayerId];
            return new PlayerData(Local.PlayerId);
        }
    }
    private static PlayerData PrivateLocalData = new();

    /// <summary>
    /// Saves the local player data to a file.
    /// </summary>
    private static void SaveLocalData()
    {
        FileSystem.Data.WriteJson("playerdata.json", LocalData);
    }

    /// <summary>
    /// Load the local player data from a file.
    /// </summary>
    private static PlayerData LoadLocalData()
    {
        var data = FileSystem.Data.ReadJsonOrDefault<PlayerData>("playerdata.json", new PlayerData());
        data.PlayerId = Local.PlayerId;
        return data;
    }

    /// <summary>
    /// Gives money to all Clients that are in the current game. This should only be called from the Server. If force is true then money is also given to clients not in the current game.
    /// </summary>
    public static void GiveAllMoney(int amount, bool force = false)
    {
        List<long> ids = new();
        List<int> moneys = new();

        var sendingClients = new List<Client>();
        foreach(var client in Client.All)
        {
            if(client.Pawn is PlatesPlayer ply && (force || ply.InGame))
            {
                PlayerData data = Players[client.PlayerId];
                data.Money += amount;

                ids.Add(data.PlayerId);
                moneys.Add(data.Money);
                
                sendingClients.Add(client);
            }
        }

        ClientUpdateMoneyMultiple(ids.ToArray(), moneys.ToArray());
    }

    /// <summary>
    /// Gives money to a specific Client via PlayerId. This should only be called from the Server.
    /// </summary>
    public static void GiveMoney(long id, int amount)
    {
        PlayerData data = Players[id];
        data.Money += amount;

        ClientUpdateMoney(id, data.Money);
    }

    /// <summary>
    /// Returns true if a Client has enough money specified.
    /// </summary>
    public static bool HasMoney(long id, int amount)
    {
        PlayerData data = Players[id];
        return data.Money >= amount;
    }

    /// <summary>
    /// Attempts to spend a client's money. Returns true if the payment was successful and returns false if they could not afford it.
    /// </summary>
    public static bool SpendMoney(long id, int amount)
    {
        PlayerData data = Players[id];
        if(data.Money >= amount)
        {
            data.Money -= amount;
            ClientUpdateMoney(id, data.Money);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns the amount of money a specified Client has.
    /// </summary>
    public static int GetMoney(long id)
    {
        return Players[id].Money;
    }

    public static void AddTempEntry(long id)
    {
        if(!Players.ContainsKey(id))
        {
            Players.Add(id, new PlayerData(id));
        }
    }

    public static void AddEntry(long id, PlayerData data, string dataString)
    {
        if(Players.ContainsKey(id)) Players.Remove(id);
        Players.Add(id, data);
        AddEntryLocally(id, dataString);
    }

    [ClientRpc]
    public static void AddEntryLocally(long id, string dataString)
    {
        PlayerData data = JsonSerializer.Deserialize<PlayerData>(dataString);
        if(!Players.ContainsKey(id)) Players.Add(id,  data);
    }

    [ClientRpc]
    public static void ClientUpdateMoney(long id, int money)
    {
        PlayerData data = Players[id];
        data.Money = money;
        if(id == Local.PlayerId) SaveLocalData();
    }

    [ClientRpc]
    public static void ClientUpdateMoneyMultiple(long[] ids, int[] moneys)
    {
        for(int i=0; i<ids.Length; i++)
        {
            var id = ids[i];
            PlayerData data = Players[id];
            if(data != null)
            {
                data.Money = moneys[i];
            }
            if(id == Local.PlayerId) SaveLocalData();
        }
    }

    [ClientRpc]
    public static void ClientUpdateEveryone(string dataString)
    {
        Players = JsonSerializer.Deserialize<IDictionary<long, PlayerData>>(dataString);
        SaveLocalData();
    }

    /// <summary>
    /// Tells the client to tell the server what our data is. This should only be called when a client joins once ever so don't use it pls. This *currently* isn't server authoritative in any way.
    /// </summary>
    [ClientRpc]
    public static void RequestPlayerData()
    {
        var data = LoadLocalData();
        AddPlayerDataEntry(JsonSerializer.Serialize(data));
    }


    [ConCmd.Server]
	private static void AddPlayerDataEntry(string dataString)
	{
		PlayerData data = JsonSerializer.Deserialize<PlayerData>(dataString);
		foreach(var client in Client.All)
		{
			if(client.PlayerId == data.PlayerId)
			{
				PlayerDataManager.AddEntry(data.PlayerId, data, dataString);
				break;
			}
		}
	}

}

public class PlayerData : BaseNetworkable
{
    public long PlayerId {get;set;}
    public int Money {get;set;}

    public PlayerData(){}
    public PlayerData(long id = 0, int money = 0)
    {
        PlayerId = id;
        Money = money;
    }
}
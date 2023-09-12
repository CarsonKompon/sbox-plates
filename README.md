<p align="center">
  <img src="https://i.imgur.com/SAKDEGU.png">
  <p align="center">A game of chance inspired by <a href="https://www.roblox.com/games/564086481/Plates-of-Fate-Mayhem">Plates of Fate: Mayhem</a> and <a href="https://www.roblox.com/games/4783966408/Plates-of-Fate-Remastered">Plates of Fate: Remastered</a>
</p>

# How to make your own Events

Here's an example of an empty event class that does nothing
```c#
public class ExampleEvent : PlatesEvent
{
    public ExampleEvent(){
        name = "example_event";
        text = " plate(s) will do nothing in ";
        type = EventType.Plate;
    }
}
```


`name` - The name of the event for use with the `plates_event` console command

`text` - The text shown when the event is counting down

`type` - Can be `EventType.Plate`, `EventType.Player` or `EventType.Arena` depending on how you want your event to act (More details below)

## Plate Events

When `type = EventType.Plate`, the affected Plate is highlighted and the `OnEvent(Plate plate)` function is called.

```c#
 
public class PlateGrow10Event : PlatesEvent
{
    public PlateGrow10Event(){
        name = "plate_grow_10";
        text = " plate(s) will grow 10% in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.Grow(0.10f);
    }
}
```

## Player Events

When `type = EventType.Player`, the affected Player is highlighted and the `OnEvent(Entity entity)` function is called.

```c#
 
public class PlayerGrowEvent : PlatesEvent
{
    public PlayerGrowEvent(){
        name = "player_grow";
        text = " player(s) will grow in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity entity){
        entity.Scale += 0.1f;
    }
}
```

## Arena Events

When `type = EventType.Arena`, nothing is highlighted and the `OnEvent()` function is called.

Because nothing is highlighted, you'll have to apply glow to the entities you affect yourself (if any)

```c#
 
public class ArenaPlateGrow10Event : PlatesEvent
{
    public ArenaPlateGrow10Event(){
        name = "arena_grow_10";
        text = "All plates will grow 10% in ";
        type = EventType.Arena;
        minAffected = 1;
        maxAffected = 1;
    }

    public override void OnEvent(){
        foreach(var plate in Entity.All.OfType<Plate>()){
            plate.Grow(0.10f);
            plate.GlowActive = true;
            plate.GlowColor = Color.Blue;
        }
    }
}
```

`minAffected` - The minimum amount of times the event should trigger

`maxAffected` - The maximum amount of times the event should trigger

(Arena events typically have both set to `1` but you can set them to whatever you want in any other event type)

# How to make your own Round Type

Here's an example of a round type that does nothing

```c#
public class BigPlatesRoundType : PlatesRound
{
    public BigPlatesRoundType(){
        name = "Nothing";
        command = "round_nothing";
    }

    public override void OnEvent(){
        Log.Info("Nothing happened");
    }
}
```

`name` - The name of the round shown in-game

`command` - The name of the round for use in the `plates_round` console command

`OnEvent()` - This function is called at the start of the game after all plates have been initialized

# Console Commands

These are useful debug tools for testing newly added events and round types.

`plates_start` - Force starts a new game. Don't use this while a game is already running.

`plates_event <event_name>` - Sets the next event in the current game.

`plates_round <round_name>` - Sets the round type of the next game.

# Other Useful Information

If you're creating a custom Entity that you want to be properly cleaned up on Game End, add this to your entity's constructor:
```c#
PlatesGame.AddEntity(this);
```
or when instantiating your entity, you can add it to a plate's entity list so it's cleaned up when the plate is destroyed:
```c#
public override void OnEvent(Plate plate){
    var ent = new PlateGrowInfinitelyEnt(plate);
    plate.AddEntity(ent);
}
```

If you're making custom UI for one of your events, create a class that looks something like this:
```c#
[LoadUI]
public class ExampleUI : Panel
{
}
```
The class must derive `Panel` and must have `[LoadUI]` the line above to be added to the RootPanel on game start.

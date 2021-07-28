# Pl&tes
A game of chance inspired by [Plates of Fate: Mayhem](https://www.roblox.com/games/564086481/Plates-of-Fate-Mayhem) and [Plates of Fate: Remastered](https://www.roblox.com/games/4783966408/Plates-of-Fate-Remastered)


# How to make your own Events

Here's an example of an empty event class that does nothing
```c#
[EventBase]
public class ExampleEvent : EventBase
{
    public ExampleEvent(){
        name = "example_event";
        text = " plate(s) will do nothing in ";
        type = EventType.Plate;
    }
}
```
**NOTE: DO NOT REMOVE `[EventBase]`!!! THIS IS HOW WE LOAD THE CLASS AUTOMAGICALLY ON GAME START/HOTLOAD**


`name` - The name of the event for use with the `plates_event` console command

`text` - The text shown when the event is counting down

`type` - Can be `EventType.Plate`, `EventType.Player` or `EventType.Arena` depending on how you want your event to act (More details below)

## Plate Events

When `type = EventType.Plate`, the affected Plate is highlighted and the `OnEvent(Plate plate)` function is called.

```c#
[EventBase]
public class PlateGrow10Event : EventBase
{
    public PlateGrow10Event(){
        name = "plate_grow_10";
        text = " plate(s) will grow 10% in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.toScale += 0.10f;
    }
}
```

## Player Events

When `type = EventType.Player`, the affected Player is highlighted and the `OnEvent(Entity entity)` function is called.

```c#
[EventBase]
public class PlayerGrowEvent : EventBase
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
[EventBase]
public class ArenaPlateGrow10Event : EventBase
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
            plate.toScale += 0.10f;
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
[RoundTypeBase]
public class BigPlatesRoundType : RoundTypeBase
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


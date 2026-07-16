# FaF.Data

This directory contains the FaF Data system. In FaF, most content is declared as such:

```
{content_medium}: (Worlds, Maps, Items, Gears, ect) (these might be inside other types of content)
    {content_id}: (WoodenPallet,SolidSteelPallet,WeldGun,WireGun,ect):
        {content_medium}Meta.tres ({content_medium}Registry Resource)
        -- THE NEXT FOLDERS ARE OPTIONAL
        Data:
            RECURSIVE (same thing as this)
```



The current organization of FaF's content is as such:

```
PROJECT_ROOT/Data:
    Worlds:
        *:
            WorldMeta.tres

            Assets:
                -- assets folder for textures, sounds, ect relating to the content.

            Data:
                Maps:
                    *:
                        MapMeta.tres (MapRegistry Resource)

                        -- THIS IS REQUIRED TO INHERIT FROM PROJECT_ROOT/Data/Worlds/Base/Data/Maps/Base_Map/Base_Map.tscn
                        name_doesnt_matter.tscn (PackedScene) (recommend to name it after the map so godot shows the correct name in the editor)
                        
                        -- IF YOUR USING TrenchBroom / OTHER MAPPING SOFTWARE
                        name_doesnt_matter.map (Map file or other formats) (same recommendation for the naming)

                        Assets:
                            -- assets folder for textures, sounds, ect relating to the content
                Items:
                    *:
                        ItemMeta.tres (ItemRegistry Resource)

                        -- THIS IS REQUIRED TO INHERIT FROM PROJECT_ROOT/Data/Worlds/Base/Data/Items/Base_Item/Base_Item.tscn
                        name_doesnt_matter.tscn (PackedScene) (recommend to name it after the itemID so godot shows the correct name in the editor)

                        Assets:
                            -- assets folder for textures, sounds, ect relating to the content
                Gears:
                    *:
                        GearMeta.tres (GearRegistry Resource)

                        -- THIS IS REQUIRED TO INHERIT FROM PROJECT_ROOT/Data/Worlds/Base/Data/Gears/Base_Gear/Base_Gear.tscn
                        name_doesnt_matter.tscn (PackedScene) (recommend to name it after the gearID so godot shows the correct name in the editor)
        
        Global:
            -- This is a special required world not shown in game that artificially adds its content to every other world. Worlds can choose to disable all or parts of globals content.
        Base:
            -- This is a special required world not shown in game that stores the base scenes for each type of registry. These peices of content all are not real and do not show in game.

    RegistryObjects (registry classes, just ignore this)
```
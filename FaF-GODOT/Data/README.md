# FaF.Data

This directory contains the FaF Data system. In FaF, most content is declared as such:

```
{content_medium}: (Worlds, Maps, Props, Gears, ect) (these might be inside other types of content)
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

            -- THIS IS REQUIRED TO INHERIT FROM PROJECT_ROOT/Game/World/BaseMap.tscn
            -- OPTIONAL: Worlds don't need to have a map in them in the case that they are meant to only add items (such as the FaF world). Should be used alongside ShowInGame:false.
            name_doesnt_matter.tscn (WorldRoot PackedScene) (recommend to name it after the world so godot shows the correct name in the editor)

            Assets:
                -- assets folder for textures, sounds, ect relating to the content.

            Data:
                Maps:
                    *:
                        MapMeta.tres (MapRegistry Resource)

                        -- THIS IS REQUIRED TO INHERIT FROM PROJECT_ROOT/Game/World/BaseMap.tscn
                        name_doesnt_matter.tscn (MapRoot PackedScene) (recommend to name it after the map so godot shows the correct name in the editor)
                        
                        -- IF YOUR USING TrenchBroom / OTHER MAPPING SOFTWARE
                        name_doesnt_matter.map (Map file or other formats) (same recommendation for the naming)

                        Assets:
                            -- assets folder for textures, sounds, ect relating to the content
                Props:
                    *:
                        PropMeta.tres (PropRegistry Resource)

                        -- THIS IS REQUIRED TO INHERIT FROM PROJECT_ROOT/Game/Prop/BaseProp.tscn
                        name_doesnt_matter.tscn (PackedScene) (recommend to name it after the propID so godot shows the correct name in the editor)

                        Assets:
                            -- assets folder for textures, sounds, ect relating to the content
                Gears:
                    *:
                        GearMeta.tres (GearRegistry Resource)

                        -- THIS IS REQUIRED TO INHERIT FROM PROJECT_ROOT/Game/Gear/BaseGear.tscn
                        name_doesnt_matter.tscn (PackedScene) (recommend to name it after the gearID so godot shows the correct name in the editor)
                
                -- This is a category of registries.
                AvatarItems:
                    Accessories:
                        *:
                            AccessoryMeta.tres (AccessoryRegistry Resource)

                            name_doesnt_matter.tscn (Accessory PackedScene)

                            Assets:
                                -- assets folder for resources relating to the content.
                    
                    Pants:
                        *:
                            PantsMeta.tres (PantsRegistry Resource)

                            LeftLeg.png/.jpg/ect (Image) (Use the leg template for this!)
                            RightLeg.png/.jpg/ect (Image) (Use the leg template for this!)
                    
                    TShirts:
                        *:
                            TShirtMeta.tres (TShirtRegistry Resource)

                            Torso.png/.jpg/ect (Image) (Any image! No template required. Stuck ontop of the Torso)
                    
                    Shirts:
                        *:
                            ShirtMeta.tres (ShirtRegistry Resource)

                            LeftArm.png/.jpg/ect (Image) (Use the arm template for this!)
                            RightArm.png/.jpg/ect (Image) (Use the arm template for this!)
                            Torso.png/.jpg/ect (Image) (Use the torso template for this!)

    RegistryObjects (registry classes, just ignore this)
```
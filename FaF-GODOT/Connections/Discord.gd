# The discord rpc extension only supports GDScript. Quite annoying as now I have to provide bridge functions in GameManager for content & static properties.

extends Node

func reset_rpc():
    DiscordRPC.details = "Fling And Fight"
    DiscordRPC.state = "Idling..."
    DiscordRPC.large_image = "symbol_icon"
    DiscordRPC.large_image_text = "Fling And Fight"
    DiscordRPC.small_image = "title_icon"
    DiscordRPC.start_timestamp = int(Time.get_unix_time_from_system())
    DiscordRPC.party_id = ""
    DiscordRPC.current_party_size = 0
    DiscordRPC.max_party_size = 0

func _ready() -> void:
    # base configuration

    DiscordRPC.app_id = 1535523972593094698
    reset_rpc()

    # connections

    GameManager.connect("SwitchedToTitleScreen", _on_switched_to_title_screen)
    GameManager.connect("JoinedServer", _on_joined_server)

func _on_switched_to_title_screen() -> void:
    reset_rpc()

    DiscordRPC.details = "Fling And Fight"
    DiscordRPC.state = "On title screen..."

    DiscordRPC.refresh()

func _on_joined_server() -> void:
    reset_rpc()

    var worldRegistry: WorldRegistry = GameManager.ContentLoaderGetWorld(GameManager.GetLoadedWorld())

    DiscordRPC.details = "Playing " + worldRegistry.Name + " by " + worldRegistry.Creator
    DiscordRPC.state = "In a game"

    DiscordRPC.max_party_size = 32
    DiscordRPC.current_party_size = 1

    DiscordRPC.party_id = GameManager.GetGlobalServerID()

    DiscordRPC.refresh()

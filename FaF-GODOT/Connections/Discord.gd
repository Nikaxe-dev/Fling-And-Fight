# The discord rpc extension only supports GDScript. Quite annoying as now I have to provide bridge functions in GameManager for content & static properties.

extends Node

@onready var NetworkManager: Node = GameManager.get_node("NetworkManager")

func reset_rpc():
	DiscordRPC.details = "Fling And Fight"
	DiscordRPC.state = ""
	DiscordRPC.large_image = "symbol_icon"
	DiscordRPC.large_image_text = "Fling And Fight"
	DiscordRPC.small_image = "title_icon"
	DiscordRPC.party_id = ""
	DiscordRPC.current_party_size = 0
	DiscordRPC.max_party_size = 32

func _ready() -> void:
	# base configuration

	DiscordRPC.start_timestamp = int(Time.get_unix_time_from_system())
	DiscordRPC.app_id = 1535523972593094698

	reset_rpc()

	# connections

	GameManager.connect("FaFEditorLoaded", _on_editor_loaded)

	GameManager.connect("SwitchedToTitleScreen", _on_switched_to_title_screen)
	GameManager.connect("JoinedServer", _on_joined_server)

	NetworkManager.connect("PlayerConnected", update_player_count)
	NetworkManager.connect("PlayerDisconnected", update_player_count)

func _on_switched_to_title_screen() -> void:
	reset_rpc()

	DiscordRPC.details = "Title screen"

	DiscordRPC.refresh()

func update_player_count(_peer_id: int = 0) -> void:
	DiscordRPC.current_party_size = multiplayer.get_peers().size()
	DiscordRPC.state =  ("Multiplayer" if DiscordRPC.current_party_size > 1 else "Singleplayer") + " game"

	DiscordRPC.refresh()

func _on_editor_loaded() -> void:
	reset_rpc()

	DiscordRPC.details = "In the editor"

	DiscordRPC.refresh()

func _on_joined_server() -> void:
	reset_rpc()

	var worldRegistry = GameManager.ContentLoaderGetMod(GameManager.GetLoadedWorld())

	DiscordRPC.details = worldRegistry.Name + " by " + worldRegistry.Creator

	update_player_count()

	DiscordRPC.party_id = GameManager.GetGlobalServerID()

	DiscordRPC.refresh()

using System;
using FaF.Core.Tree.Game;
using FaF.Core.Tree.Game.Implementations;
using Godot;

namespace FaF.Core.Extensions;

public static class NodeTreeAccessExtensions
{
    public static T GetNodeRecursive<T>(this Node node, Exception customException = null)
    {
        customException ??= new TreeNodeNotFoundException(node, typeof(T).Name);

        Node current = node;
        while (current != null)
        {
            if (current is T result)
                return result;
            current = current.GetParent();
        }

        throw customException;
    }

    public static GameInstance GetGame(this Node node)
        => GetNodeRecursive<GameInstance>(node, new GameNotFoundException(node));

    public static IClientInstance GetClient(this Node node)
        => (IClientInstance)GetGame(node);
    
    public static IServerInstance GetServer(this Node node)
        => (IServerInstance)GetGame(node);

    public class TreeNodeNotFoundException(Node node, string keyword) : Exception($"Failed to find {keyword}: {node.GetPath()} is not under a {keyword}.");

    public class GameNotFoundException(Node node) : TreeNodeNotFoundException(node, "game");
    public class WorldRootNotFoundException(Node node) : TreeNodeNotFoundException(node, "world root");
    public class MapNotFoundException(Node node) : TreeNodeNotFoundException(node, "map");
}
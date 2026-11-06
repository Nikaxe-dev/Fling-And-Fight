using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace FaF.Core.Lib;

public struct Version()
{
    public string Name;

    public bool AnyEnabled = false;

    public int Major;
    public int Minor;
    public int Patch;

    public static Version Parse(string text)
    {
        string[] numbers = text.Split('.');
        
        return new()
        {
            Major = numbers.Length >= 1 ? int.Parse(numbers[0]) : 0,
            Minor = numbers.Length >= 2 ? int.Parse(numbers[1]) : 0,
            Patch = numbers.Length >= 3 ? int.Parse(numbers[2]) : 0
        };
    }

    private static List<string[]> ParseRequirements(string requirements)
    {
        List<string[]> result = [];

        string[] ORs = requirements.Split('|');

        foreach (string OR in ORs)
            result.Add(OR.Split(' '));
        
        return result;
    }

    /// <summary>
    /// Requirements are formmated as for example: <br/>
    /// >=1.0.5 <2.0.0|>=0.5.0 <=0.6.0 <br/>
    /// This meaning either between 1.0.5 and 2.0.0 or between 0.5.0 and 0.6.0. <br/>
    /// OR is last in the order of operations.
    /// </summary>
    public readonly bool MatchesRequirements(string requirements)
    {
        if (requirements == "any")
            return true;

        var parsedRequirements = ParseRequirements(requirements);

        foreach (string[] ANDs in parsedRequirements)
        {
            bool andResult = true;

            foreach (string requirement in ANDs)
            {
                Version version = Parse(requirement.TrimPrefix(">=").TrimPrefix("<=").TrimPrefix("==").TrimPrefix("!="));
                
                if (requirement.StartsWith("=="))
                    andResult &= this == version;
                else if (requirement.StartsWith("!="))
                    andResult &= this != version;
                else if (requirement.StartsWith(">="))
                    andResult &= this >= version;
                else if (requirement.StartsWith("<="))
                    andResult &= this <= version;
                else if (requirement.StartsWith(">"))
                    throw new InvalidOperationException("< is not a supported requirement operation for mod versions.");
                else if (requirement.StartsWith("<"))
                    throw new InvalidOperationException("> is not a supported requirement operation for mod versions.");
                else
                    andResult &= this == version;
                
                if (!andResult)
                    break;
            }
            
            if (andResult)
                return true;
        }

        return false;
    }

    public static Version GetRecommendedVersion(string requirements)
    {
        if (requirements == "any")
            return new Version {AnyEnabled = true};

        var parsedRequirements = ParseRequirements(requirements);
        
        Version highestVersion = default;

        foreach (string[] ANDs in parsedRequirements)
        {
            List<Version> unequalTo = [];
            List<Version> equalTo = [];

            Version? exactEqualTo = null;

            Version? maximum = null;
            Version? minimum = null;

            foreach (string requirement in ANDs)
            {
                Version version = Parse(requirement.TrimPrefix(">=").TrimPrefix("<=").TrimPrefix("==").TrimPrefix("!=").TrimPrefix(">").TrimPrefix("<"));

                if (requirement.StartsWith("=="))
                    exactEqualTo = version;
                else if (requirement.StartsWith("!="))
                    unequalTo.Add(version);
                else if (requirement.StartsWith(">="))
                {
                    equalTo.Add(version);
                    if (minimum == null || version < minimum)
                        minimum = version;
                } else if (requirement.StartsWith("<="))
                {
                    equalTo.Add(version);
                    if (maximum == null || version > maximum)
                        maximum = version;
                } else if (requirement.StartsWith(">"))
                    throw new InvalidOperationException("> is not a supported requirement operation for mod versions.");
                else if (requirement.StartsWith("<"))
                    throw new InvalidOperationException("< is not a supported requirement operation for mod versions.");
                else
                    exactEqualTo = version;
            }

            Version localHighestVersion = default;

            if (exactEqualTo != null && !unequalTo.Contains((Version)exactEqualTo))
                localHighestVersion = (Version)exactEqualTo;
            
            if (maximum != null && maximum > localHighestVersion && !unequalTo.Contains((Version)maximum))
                localHighestVersion = (Version)maximum;
            
            if (minimum != null && minimum > localHighestVersion && !unequalTo.Contains((Version)minimum))
                localHighestVersion = (Version)minimum;
            
            foreach (Version version in equalTo)
                if (version > localHighestVersion && !unequalTo.Contains(version))
                    localHighestVersion = version;
            
            if (localHighestVersion > highestVersion)
                highestVersion = localHighestVersion;
        }

        return highestVersion;
    }

    public static bool operator ==(Version left, Version right)
        => left.Major == right.Major && left.Minor == right.Minor && left.Patch == right.Patch;
    
    public static bool operator !=(Version left, Version right)
        => left.Major != right.Major && left.Minor != right.Minor && left.Patch != right.Patch;
    
    public static bool operator >(Version left, Version right)
        => (left.Major == right.Major && left.Minor == right.Minor && left.Patch > right.Patch) || (left.Major == right.Major && left.Minor > right.Minor) || (left.Major > right.Major);

    public static bool operator <(Version left, Version right)
        => (left.Major == right.Major && left.Minor == right.Minor && left.Patch < right.Patch) || (left.Major == right.Major && left.Minor < right.Minor) || (left.Major < right.Major);
    
    public static bool operator >=(Version left, Version right)
        => (left.Major == right.Major && left.Minor == right.Minor && left.Patch >= right.Patch) || (left.Major == right.Major && left.Minor >= right.Minor) || (left.Major >= right.Major);
    
    public static bool operator <=(Version left, Version right)
        => (left.Major == right.Major && left.Minor == right.Minor && left.Patch <= right.Patch) || (left.Major == right.Major && left.Minor <= right.Minor) || (left.Major <= right.Major);

    public override readonly bool Equals([NotNullWhen(true)] object obj)
        => obj is Version version && this == version;

    public override readonly int GetHashCode()
        => HashCode.Combine(Major, Minor, Patch);

    public override readonly string ToString()
        => AnyEnabled ? "any" : $"{Major}.{Minor}{(Patch != 0 ? $".{Patch}" : "")}";
    
    public readonly string ToDescriptiveString()
        => AnyEnabled ? "any" : $"V{Major}.{Minor}{(Patch != 0 ? $".{Patch}" : "")}{(Name != null ? $" - {Name}" : "")}";
}
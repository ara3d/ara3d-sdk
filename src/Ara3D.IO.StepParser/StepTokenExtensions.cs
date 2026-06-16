using Ara3D.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ara3D.IO.StepParser;

public static unsafe class StepTokenExtensions
{
    // A "simple entity" is a Step Entity with one attribute. If there are more attributes we ignore the rest.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (string, StepToken) AsSimpleEntity(this StepToken token, StepDocument doc)
    {
        Debug.Assert(token.IsEntity);
        var name = token.ToString();
        var listToken = token.NextToken(doc);
        var list = listToken.AsSpan(doc);
        Debug.Assert(list.Length >= 1);
        return (name, list[0]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StepToken NextToken(this StepToken token, StepDocument doc)
        => doc.Tokens[token.Index + 1];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<StepToken> AsSpan(this StepToken token, StepDocument doc)
    {
        if (token.IsUnassignedOrRedeclared)
        {
            return default;
        }

        Debug.Assert(token.IsList);
        // We want to get the index of the first token in the list .
        var index = token.Index;
        Debug.Assert(index >= 0);
        
        // If the list token was not 
        Debug.Assert(token.Length >= 0);
        
        var ptr = doc.Tokens.Ptr + index;

        // Should be the same token 
        Debug.Assert(ptr->Equals(token));

        return new ReadOnlySpan<StepToken>(ptr + 1, token.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(this StepToken token, StepDocument doc)
        => token.IsList 
            ? $"({token.AsList(doc).Select(t => t.ToString(doc)).JoinStringsWithComma()})" 
            : token.ToString();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IReadOnlyList<StepToken> AsList(this StepToken token, StepDocument doc)
    {
        var input = token.AsSpan(doc);
        var output = new List<StepToken>();
        var i = 0;
        while (i < input.Length)
        {
            var element = input[i];
            output.Add(element);
            if (element.IsList)
            {
                i += element.Length + 1;
            }
            else if (element.IsEntity)
            {
                i += 1;
                if (i >= input.Length)
                    throw new Exception("Out of bounds");
                var listToken = input[i];
                if (!listToken.IsList)
                    throw new Exception("Expected a list to follow an entity");
                i += listToken.Length + 1;
            }
            else
            {
                i++;
            }
        }
        return output;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double[] AsNumbers(this StepToken value, StepDocument doc)
    {
        var vals = value.AsSpan(doc);
        var r = new double[vals.Length];
        for (var i = 0; i < vals.Length; ++i)
            r[i] = vals[i].AsNumber();
        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int[] AsIds(this StepToken value, StepDocument doc)
    {
        var vals = value.AsSpan(doc);
        var r = new int[vals.Length];
        for (var i = 0; i < vals.Length; ++i)
            r[i] = vals[i].AsId();
        return r;
    }
}
using System;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// The sole creators of <see cref="Transaction"/>, <see cref="TransactionGroup"/>, and
/// <see cref="SubTransaction"/> in this library (P1 query/mutation firewall) — every other
/// mutation method documents "requires an open transaction" and lets Revit throw if one
/// isn't. Every combinator here starts, and on success commits (or assimilates); on
/// exception it rolls back whatever it opened and rethrows — nothing is swallowed (P8).
/// </summary>
public static class Transactions
{
    /// <summary>Runs <paramref name="action"/> inside a <see cref="Transaction"/> named <paramref name="name"/>.</summary>
    public static void Run(this Document doc, string name, Action<Document> action)
        => RunCore(doc, name, AsFunc(action), default);

    /// <summary>Runs <paramref name="action"/> inside a <see cref="Transaction"/> and returns its result.</summary>
    public static T Run<T>(this Document doc, string name, Func<Document, T> action)
        => RunCore(doc, name, action, default);

    /// <summary>Runs <paramref name="action"/> with a custom <see cref="IFailuresPreprocessor"/> applied to the transaction.</summary>
    public static void Run(this Document doc, string name, Action<Document> action, IFailuresPreprocessor failures)
        => RunCore(doc, name, AsFunc(action), new TransactionOptions(failures));

    /// <summary>Runs <paramref name="action"/> with an explicit failure-handling policy (P8: no side channel).</summary>
    public static void Run(this Document doc, string name, Action<Document> action, TransactionOptions options)
        => RunCore(doc, name, AsFunc(action), options);

    /// <summary>
    /// Runs <paramref name="action"/> inside a <see cref="TransactionGroup"/>: every transaction
    /// <paramref name="action"/> opens (typically via <see cref="Run(Document, string, Action{Document})"/>)
    /// is assimilated into one undo entry named <paramref name="name"/> on success.
    /// </summary>
    public static void RunGroup(this Document doc, string name, Action<Document> action)
    {
        using var group = new TransactionGroup(doc, name);
        group.Start();
        try
        {
            action(doc);
            group.Assimilate();
        }
        catch
        {
            if (group.HasStarted() && !group.HasEnded())
                group.RollBack();
            throw;
        }
    }

    /// <summary>
    /// Runs <paramref name="action"/> inside a <see cref="SubTransaction"/>. Requires an
    /// already-open outer <see cref="Transaction"/> — Revit throws its own exception if there isn't one.
    /// </summary>
    public static void RunSub(this Document doc, Action<Document> action)
    {
        using var sub = new SubTransaction(doc);
        sub.Start();
        try
        {
            action(doc);
            sub.Commit();
        }
        catch
        {
            if (sub.HasStarted() && !sub.HasEnded())
                sub.RollBack();
            throw;
        }
    }

    static T RunCore<T>(Document doc, string name, Func<Document, T> action, TransactionOptions options)
    {
        using var tx = new Transaction(doc, name);
        tx.Start();
        ApplyFailureHandling(tx, options);
        try
        {
            var result = action(doc);
            tx.Commit();
            return result;
        }
        catch
        {
            if (tx.HasStarted() && !tx.HasEnded())
                tx.RollBack();
            throw;
        }
    }

    static void ApplyFailureHandling(Transaction tx, TransactionOptions options)
    {
        if (options.FailuresPreprocessor is null && !options.SwallowWarnings)
            return;

        var handlingOptions = tx.GetFailureHandlingOptions();
        handlingOptions.SetFailuresPreprocessor(
            options.FailuresPreprocessor ?? new WarningSwallowingFailuresPreprocessor());
        tx.SetFailureHandlingOptions(handlingOptions);
    }

    static Func<Document, object?> AsFunc(Action<Document> action)
        => doc => { action(doc); return null; };
}

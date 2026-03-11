using FreeKassaPayOnline.Model;
using MvvmNavigationLib.Services;
using Refit;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using VolgaTermalBathsEnter.Models.Interfaces;
using VolgaTermalBathsEnter.ViewModels.Popups;

namespace VolgaTermalBathsEnter.Utilities;

public static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items) => items.ForEach(source.Add);


    public static void AddWhere<T>(this ICollection<T> source, IEnumerable<T> items, Func<T, bool> predicate) =>
        items.ForEachWhere(source.Add, predicate);

    public static void RemoveRange<T>(this ICollection<T> source, IEnumerable<T> items) =>
        items.ForEach(i => source.Remove(i));

    public static void RemoveWhere<T>(this ICollection<T> source, IEnumerable<T> items, Func<T, bool> predicate) =>
        items.ForEachWhere(i => source.Remove(i), predicate);

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source) action(item);
    }

    public static void ForEachWhere<T>(this IEnumerable<T> source, Action<T> action, Func<T, bool> predicate)
    {
        foreach (var item in source)
            if (predicate(item))
                action(item);
    }

  
}
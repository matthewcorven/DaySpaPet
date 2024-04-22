using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;

namespace DaySpaPet.SharedKernel.GuardClauses;

public static class DateOnlyGuardExtensions
{
  #region DateOnlyIsNotBefore

  /// <summary>
  /// Throws an <see cref="ArgumentException"/> if <paramref name="input"/> is negative or zero.
  /// </summary>
  /// <param name="guardClause"></param>
  /// <param name="input"></param>
  /// <param name="compareToDate"></param>
  /// <param name="inputParameterName"></param>
  /// <param name="message">Optional. Custom error message</param>
  /// <returns><paramref name="input" /> if the value is before <paramref name="compareToDate" />.</returns>
  /// <exception cref="ArgumentException"></exception>
  public static DateOnly DateOnlyIsNotBefore(this IGuardClause guardClause,
      DateOnly input,
      DateOnly compareToDate,
      [CallerArgumentExpression(nameof(input))] string? inputParameterName = null,
      string? message = null)
  {
    return DateOnlyIsNotBefore<DateOnly>(guardClause, input, compareToDate, inputParameterName, message);
  }

  /// <summary>
  /// Throws an <see cref="ArgumentException"/> if <paramref name="input"/> is negative or zero. 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="guardClause"></param>
  /// <param name="input"></param>
  /// <param name="compareToDate"></param>
  /// <param name="inputParameterName"></param>
  /// <param name="message">Optional. Custom error message</param>
  /// <returns><paramref name="input" /> if the value is before <paramref name="compareToDate" />.</returns>
  /// <exception cref="ArgumentException"></exception>
#pragma warning disable IDE0060 // Remove unused parameter
  private static T DateOnlyIsNotBefore<T>(this IGuardClause guardClause,
#pragma warning restore IDE0060 // Remove unused parameter
      T input,
      T compareToDate,
      [CallerArgumentExpression(nameof(input))] string? inputParameterName = null,
      string? message = null) where T : struct, IComparable
  {
    if (input.CompareTo(compareToDate) < 0)
    {
      var comparedToText = compareToDate.Equals(DateOnly.FromDateTime(DateTime.Today)) ? "today" : compareToDate.ToString();
      throw new ArgumentException(message ?? $"Date must not be before {comparedToText}.", inputParameterName);
    }

    return input;
  }

  #endregion

  #region DateOnlyIsNotAfter

  /// <summary>
  /// Throws an <see cref="ArgumentException"/> if <paramref name="input"/> is negative or zero.
  /// </summary>
  /// <param name="guardClause"></param>
  /// <param name="input"></param>
  /// <param name="compareToDate"></param>
  /// <param name="inputParameterName"></param>
  /// <param name="message">Optional. Custom error message</param>
  /// <returns><paramref name="input" /> if the value is after <paramref name="compareToDate" />.</returns>
  /// <exception cref="ArgumentException"></exception>
  public static DateOnly DateOnlyIsNotAfter(this IGuardClause guardClause,
      DateOnly input,
      DateOnly compareToDate,
      [CallerArgumentExpression(nameof(input))] string? inputParameterName = null,
      string? message = null)
  {
    return DateOnlyIsNotAfter<DateOnly>(guardClause, input, compareToDate, inputParameterName, message);
  }

  /// <summary>
  /// Throws an <see cref="ArgumentException"/> if <paramref name="input"/> is negative or zero. 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="guardClause"></param>
  /// <param name="input"></param>
  /// <param name="compareToDate"></param>
  /// <param name="inputParameterName"></param>
  /// <param name="message">Optional. Custom error message</param>
  /// <returns><paramref name="input" /> if the value is after <paramref name="compareToDate" />.</returns>
  /// <exception cref="ArgumentException"></exception>
#pragma warning disable IDE0060 // Remove unused parameter
  private static T DateOnlyIsNotAfter<T>(this IGuardClause guardClause,
#pragma warning restore IDE0060 // Remove unused parameter
      T input,
      T compareToDate,
      [CallerArgumentExpression(nameof(input))] string? inputParameterName = null,
      string? message = null) where T : struct, IComparable
  {
    if (input.CompareTo(compareToDate) > 0)
    {
      var comparedToText = compareToDate.Equals(DateOnly.FromDateTime(DateTime.Today)) ? "today" : compareToDate.ToString();
      throw new ArgumentException(message ?? $"Date must not be after {comparedToText}.", inputParameterName);
    }

    return input;
  }

  #endregion
}

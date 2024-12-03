using UnityEngine;
using JamForge.Monad;

/// <summary>
/// Demonstrates practical usage of monadic types (Optional and Either) in game development.
/// These patterns help handle null values and error cases more elegantly.
/// 
/// Key concepts demonstrated:
/// - Optional: For safe handling of nullable values
/// - Either: For explicit error handling without exceptions
/// - Monad chaining: Composing operations safely
/// </summary>
public class MonadExample : MonoBehaviour
{
    private void Start()
    {
        // Optional Examples
        OptionalExamples();

        // Either Examples
        EitherExamples();
    }

    /// <summary>
    /// Demonstrates Optional monad usage for null-safe operations.
    /// Optional is perfect for:
    /// - Handling potentially missing game objects or components
    /// - Processing user input that might be empty
    /// - Chaining operations that might fail
    /// </summary>
    private void OptionalExamples()
    {
        // Example 1: Creating Optionals
        // Some() wraps an existing value
        // None() represents absence of value
        var playerName = Optional<string>.Some("Hero");
        var emptyName = Optional<string>.None();

        // Example 2: Safe value access using pattern matching
        // Match() lets us handle both cases (value present/absent) elegantly
        playerName.Match(
            some: pName => Debug.Log($"Player name is: {pName}"),
            none: () => Debug.Log("No player name set")
        );

        // Example 3: Transform Optional values safely
        // Map() transforms the value if present, preserves None if absent
        var nameLength = playerName.Map(pName => pName.Length);
        Debug.Log($"Name length: {nameLength.GetOrDefault()}");

        // Example 4: Chain Optional operations
        // We can chain multiple operations safely:
        // 1. Filter(): Only proceed if condition is met
        // 2. Map(): Transform the value
        // GetOrDefault(): Provide fallback if any step fails
        var upperName = playerName
            .Filter(name => name.Length > 0) // Only proceed if name isn't empty
            .Map(name => name.ToUpper());    // Convert to uppercase if we got here

        Debug.Log($"Upper name: {upperName.GetOrDefault("NO NAME")}");
    }

    /// <summary>
    /// Demonstrates Either monad usage for explicit error handling.
    /// Either is perfect for:
    /// - Parsing user input with specific error messages
    /// - Game mechanics that can fail in different ways
    /// - API calls that need detailed error reporting
    /// </summary>
    private void EitherExamples()
    {
        // Example 1: Basic Either usage for parsing
        // Either lets us handle success and error cases explicitly
        var parsedNumber = ParseNumber("123");
        var parsedString = parsedNumber.Match(
            right: number =>
            {
                Debug.Log($"Parsed number: {number}");
                return $"Parsed number: {number}";
            },
            left: error =>
            {
                Debug.Log($"Error: {error}");
                return $"Error: {error}";
            }
        );

        // Example 2: Chaining Either operations
        // We can chain multiple operations while preserving error context:
        // 1. Map(): Transform success value
        // 2. Filter(): Add validation with custom error
        var result = parsedNumber
            .Map(number => number * 2)                    // Double the number if parsing succeeded
            .Filter(
                number => number > 0,                     // Validate the result
                () => "Number must be positive"           // Provide error if validation fails
            );
        Debug.Log(result);

        // Example 3: Game mechanic with Either
        // Using Either for game mechanics that can fail
        var damageCalculation = CalculateDamage(10, 5);
        damageCalculation.Match(
            right: damage =>
            {
                ApplyDamage(damage);
                return $"Applied {damage} damage";
            },
            left: error =>
            {
                Debug.LogError(error);
                return $"Error: {error}";
            });
    }

    /// <summary>
    /// Example utility function that parses a string to integer.
    /// Returns Either:
    /// - Right: Successfully parsed integer
    /// - Left: Error message if parsing fails
    /// </summary>
    private Either<string, int> ParseNumber(string input)
    {
        if (int.TryParse(input, out var result))
        {
            return Either<string, int>.Right(result);
        }
        return Either<string, int>.Left("Failed to parse number");
    }

    /// <summary>
    /// Example game mechanic that calculates damage with validation.
    /// Returns Either:
    /// - Right: Calculated final damage
    /// - Left: Error message if parameters are invalid
    /// </summary>
    private Either<string, float> CalculateDamage(float baseDamage, float defense)
    {
        if (baseDamage <= 0)
        {
            return Either<string, float>.Left("Base damage must be positive");
        }
        if (defense < 0)
        {
            return Either<string, float>.Left("Defense cannot be negative");
        }

        var finalDamage = Mathf.Max(0, baseDamage - defense);
        return Either<string, float>.Right(finalDamage);
    }

    /// <summary>
    /// Example game mechanic that applies damage to a target.
    /// In a real game, this would modify the target's health.
    /// </summary>
    private void ApplyDamage(float damage)
    {
        Debug.Log($"Applied {damage} damage to target");
    }
}
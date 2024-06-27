namespace MemoryMosaic.Extensions;

/// <summary>
/// Provides extension methods for the UIntPtr class.
/// </summary>
public static class UIntPtrExtensions {
    /// <summary>
    /// Returns the value of the pointer as a hexadecimal string with a fixed length of 8 characters.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <returns>The value of the pointer as a hexadecimal string.</returns>
    public static string ToHex(this nuint pointer) {
        return pointer.ToUInt64().ToString("X8");
    }

    #region Methods: Arithmetics

    /// <summary>
    /// Decrements the value of the pointer by the specified 32-bit integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to decrement by.</param>
    /// <returns>The decremented pointer.</returns>
    public static nuint Decrement(this nuint pointer, int value) {
        return pointer.Increment(-value);
    }

    /// <summary>
    /// Decrements the value of the pointer by the specified 64-bit integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to decrement by.</param>
    /// <returns>The decremented pointer.</returns>
    public static nuint Decrement(this nuint pointer, long value) {
        return pointer.Increment(-value);
    }

    /// <summary>
    /// Decrements the value of the pointer by the specified UIntPtr value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to decrement by.</param>
    /// <returns>The decremented pointer.</returns>
    public static nuint Decrement(this nuint pointer, nuint value) {
        switch (nuint.Size) {
            case sizeof(uint):
                return new nuint(pointer.ToUInt32() - value.ToUInt32());

            default:
                return new nuint(pointer.ToUInt64() - value.ToUInt64());
        }
    }

    /// <summary>
    /// Increments the value of the pointer by the specified 32-bit integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to increment by.</param>
    /// <returns>The incremented pointer.</returns>
    public static nuint Increment(this nuint pointer, int value) {
        unchecked {
            switch (nuint.Size) {
                case sizeof(uint):
                    return new nuint(pointer.ToUInt32() + (uint)value);

                default:
                    return new nuint(pointer.ToUInt64() + (ulong)value);
            }
        }
    }

    /// <summary>
    /// Increments the value of the pointer by the specified 64-bit integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to increment by.</param>
    /// <returns>The incremented pointer.</returns>
    public static nuint Increment(this nuint pointer, long value) {
        unchecked {
            switch (nuint.Size) {
                case sizeof(uint):
                    return new nuint((uint)(pointer.ToUInt32() + value));

                default:
                    if (value >= 0)
                        return new nuint(pointer.ToUInt64() + (ulong)value);
                    else
                        return new nuint(pointer.ToUInt64() - (ulong)-value);
            }
        }
    }

    /// <summary>
    /// Increments the value of the pointer by the specified UIntPtr value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to increment by.</param>
    /// <returns>The incremented pointer.</returns>
    public static nuint Increment(this nuint pointer, nuint value) {
        unchecked {
            switch (nuint.Size) {
                case sizeof(uint):
                    return new nuint(pointer.ToUInt32() + value.ToUInt32());
                default:
                    return new nuint(pointer.ToUInt64() + value.ToUInt64());
            }
        }
    }

    #endregion

    #region Methods: Comparison

    /// <summary>
    /// Compares the value of the pointer to the specified 32-bit integer value.
    /// </summary>
    /// <param name="left">The pointer.</param>
    /// <param name="right">The value to compare.</param>
    /// <returns>A signed integer that indicates the relative values of the pointer and the value.</returns>
    public static int CompareTo(this nuint left, int right) {
        return left.CompareTo((uint)right);
    }

    /// <summary>
    /// Compares the value of the pointer to the specified UIntPtr value.
    /// </summary>
    /// <param name="left">The pointer.</param>
    /// <param name="right">The value to compare.</param>
    /// <returns>A signed integer that indicates the relative values of the pointer and the value.</returns>
    public static int CompareTo(this nuint left, nuint right) {
        if (left.ToUInt64() > right.ToUInt64())
            return 1;

        if (left.ToUInt64() < right.ToUInt64())
            return -1;

        return 0;
    }

    /// <summary>
    /// Compares the value of the pointer to the specified 32-bit unsigned integer value.
    /// </summary>
    /// <param name="left">The pointer.</param>
    /// <param name="right">The value to compare.</param>
    /// <returns>A signed integer that indicates the relative values of the pointer and the value.</returns>
    public static int CompareTo(this nuint left, uint right) {
        if (left.ToUInt64() > right)
            return 1;

        if (left.ToUInt64() < right)
            return -1;

        return 0;
    }

    #endregion

    #region Methods: Conversion

    /// <summary>
    /// Converts the value of the pointer to a 32-bit unsigned integer.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <returns>A 32-bit unsigned integer that represents the value of the pointer.</returns>
    public static unsafe uint ToUInt32(this nuint pointer) {
        return (uint)(void*)pointer;
    }

    /// <summary>
    /// Converts the value of the pointer to a 64-bit unsigned integer.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <returns>A 64-bit unsigned integer that represents the value of the pointer.</returns>
    public static unsafe ulong ToUInt64(this nuint pointer) {
        return (ulong)(void*)pointer;
    }

    #endregion

    #region Methods: Equality

    /// <summary>
    /// Determines whether the value of the pointer is equal to the specified 32-bit integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to compare.</param>
    /// <returns>true if the value of the pointer is equal to the value; otherwise, false.</returns>
    public static bool Equals(this nuint pointer, int value) {
        return pointer.ToUInt32() == value;
    }

    /// <summary>
    /// Determines whether the value of the pointer is equal to the specified 64-bit integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to compare.</param>
    /// <returns>true if the value of the pointer is equal to the value; otherwise, false.</returns>
    public static bool Equals(this nuint pointer, long value) {
        return pointer.ToUInt64() == (ulong)value;
    }

    /// <summary>
    /// Determines whether the value of the pointer is equal to the value of the specified pointer.
    /// </summary>
    /// <param name="left">The pointer.</param>
    /// <param name="ptr2">The pointer to compare.</param>
    /// <returns>true if the value of the pointer is equal to the value of the specified pointer; otherwise, false.</returns>
    public static bool Equals(this nuint left, nuint ptr2) {
        return left == ptr2;
    }

    /// <summary>
    /// Determines whether the value of the pointer is equal to the specified 32-bit unsigned integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to compare.</param>
    /// <returns>true if the value of the pointer is equal to the value; otherwise, false.</returns>
    public static bool Equals(this nuint pointer, uint value) {
        return pointer.ToUInt32() == value;
    }

    /// <summary>
    /// Determines whether the value of the pointer is equal to the specified 64-bit unsigned integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to compare.</param>
    /// <returns>true if the value of the pointer is equal to the value; otherwise, false.</returns>
    public static bool Equals(this nuint pointer, ulong value) {
        return pointer.ToUInt64() == value;
    }

    /// <summary>
    /// Determines whether the value of the pointer is greater than or equal to the value of the specified pointer.
    /// </summary>
    /// <param name="left">The pointer.</param>
    /// <param name="right">The pointer to compare.</param>
    /// <returns>true if the value of the pointer is greater than or equal to the value of the specified pointer; otherwise, false.</returns>
    public static bool GreaterThanOrEqualTo(this nuint left, nuint right) {
        return left.CompareTo(right) >= 0;
    }

    /// <summary>
    /// Determines whether the value of the pointer is less than or equal to the value of the specified pointer.
    /// </summary>
    /// <param name="left">The pointer.</param>
    /// <param name="right">The pointer to compare.</param>
    /// <returns>true if the value of the pointer is less than or equal to the value of the specified pointer; otherwise, false.</returns>
    public static bool LessThanOrEqualTo(this nuint left, nuint right) {
        return left.CompareTo(right) <= 0;
    }

    #endregion

    #region Methods: Logic

    /// <summary>
    /// Performs a bitwise AND operation on the value of the pointer and the specified pointer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The pointer value to perform the AND operation with.</param>
    /// <returns>A new pointer that represents the result of the bitwise AND operation.</returns>
    public static nuint And(this nuint pointer, nuint value) {
        switch (nuint.Size) {
            case sizeof(uint):
                return new nuint(pointer.ToUInt32() & value.ToUInt32());

            default:
                return new nuint(pointer.ToUInt64() & value.ToUInt64());
        }
    }

    /// <summary>
    /// Performs a bitwise NOT operation on the value of the pointer.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <returns>A new pointer that represents the result of the bitwise NOT operation.</returns>
    public static nuint Not(this nuint pointer) {
        switch (nuint.Size) {
            case sizeof(uint):
                return new nuint(~pointer.ToUInt32());

            default:
                return new nuint(~pointer.ToUInt64());
        }
    }

    /// <summary>
    /// Performs a bitwise OR operation on the value of the pointer and the specified pointer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The pointer value to perform the OR operation with.</param>
    /// <returns>A new pointer that represents the result of the bitwise OR operation.</returns>
    public static nuint Or(this nuint pointer, nuint value) {
        switch (nuint.Size) {
            case sizeof(uint):
                return new nuint(pointer.ToUInt32() | value.ToUInt32());

            default:
                return new nuint(pointer.ToUInt64() | value.ToUInt64());
        }
    }

    /// <summary>
    /// Performs a bitwise XOR operation on the value of the pointer and the specified pointer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The pointer value to perform the XOR operation with.</param>
    /// <returns>A new pointer that represents the result of the bitwise XOR operation.</returns>
    public static nuint Xor(this nuint pointer, nuint value) {
        switch (nuint.Size) {
            case sizeof(uint):
                return new nuint(pointer.ToUInt32() ^ value.ToUInt32());

            default:
                return new nuint(pointer.ToUInt64() ^ value.ToUInt64());
        }
    }

    #endregion
}
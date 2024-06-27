namespace MemoryMosaic.Extensions;

/// <summary>
/// Provides extension methods for the IntPtr class.
/// </summary>
// Reference: https://stackoverflow.com/a/14339534
public static class IntPtrExtensions {
    /// <summary>
    /// Returns the value of the pointer as a hexadecimal string with a fixed length of 8 characters.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <returns>The value of the pointer as a hexadecimal string.</returns>
    public static string ToHex(this nint pointer) {
        return pointer.ToInt64().ToString("X8");
    }

    #region Methods: Arithmetics

    /// <summary>
    /// Decrements the value of the pointer by the specified 32-bit integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to decrement by.</param>
    /// <returns>The decremented pointer.</returns>
    public static nint Decrement(this nint pointer, int value) {
        return pointer.Increment(-value);
    }

    /// <summary>
    /// Decrements the value of the pointer by the specified 64-bit integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to decrement by.</param>
    /// <returns>The decremented pointer.</returns>
    public static nint Decrement(this nint pointer, long value) {
        return pointer.Increment(-value);
    }

    /// <summary>
    /// Decrements the value of the pointer by the specified IntPtr value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to decrement by.</param>
    /// <returns>The decremented pointer.</returns>
    public static nint Decrement(this nint pointer, nint value) {
        switch (nint.Size) {
            case sizeof(int):
                return new nint(pointer.ToInt32() - value.ToInt32());

            default:
                return new nint(pointer.ToInt64() - value.ToInt64());
        }
    }

    /// <summary>
    /// Increments the value of the pointer by the specified 32-bit integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to increment by.</param>
    /// <returns>The incremented pointer.</returns>
    public static nint Increment(this nint pointer, int value) {
        unchecked {
            switch (nint.Size) {
                case sizeof(int):
                    return new nint(pointer.ToInt32() + value);

                default:
                    return new nint(pointer.ToInt64() + value);
            }
        }
    }

    /// <summary>
    /// Increments the value of the pointer by the specified 64-bit integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to increment by.</param>
    /// <returns>The incremented pointer.</returns>
    public static nint Increment(this nint pointer, long value) {
        unchecked {
            switch (nint.Size) {
                case sizeof(int):
                    return new nint((int)(pointer.ToInt32() + value));

                default:
                    return new nint(pointer.ToInt64() + value);
            }
        }
    }

    /// <summary>
    /// Increments the value of the pointer by the specified IntPtr value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to increment by.</param>
    /// <returns>The incremented pointer.</returns>
    public static nint Increment(this nint pointer, nint value) {
        unchecked {
            switch (nint.Size) {
                case sizeof(int):
                    return new nint(pointer.ToInt32() + value.ToInt32());
                default:
                    return new nint(pointer.ToInt64() + value.ToInt64());
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
    public static int CompareTo(this nint left, int right) {
        return left.CompareTo((uint)right);
    }

    /// <summary>
    /// Compares the value of the pointer to the specified IntPtr value.
    /// </summary>
    /// <param name="left">The pointer.</param>
    /// <param name="right">The value to compare.</param>
    /// <returns>A signed integer that indicates the relative values of the pointer and the value.</returns>
    public static int CompareTo(this nint left, nint right) {
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
    public static int CompareTo(this nint left, uint right) {
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
    public static unsafe uint ToUInt32(this nint pointer) {
        return (uint)(void*)pointer;
    }

    /// <summary>
    /// Converts the value of the pointer to a 64-bit unsigned integer.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <returns>A 64-bit unsigned integer that represents the value of the pointer.</returns>
    public static unsafe ulong ToUInt64(this nint pointer) {
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
    public static bool Equals(this nint pointer, int value) {
        return pointer.ToInt32() == value;
    }

    /// <summary>
    /// Determines whether the value of the pointer is equal to the specified 64-bit integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to compare.</param>
    /// <returns>true if the value of the pointer is equal to the value; otherwise, false.</returns>
    public static bool Equals(this nint pointer, long value) {
        return pointer.ToInt64() == value;
    }

    /// <summary>
    /// Determines whether the value of the pointer is equal to the value of the specified pointer.
    /// </summary>
    /// <param name="left">The pointer.</param>
    /// <param name="ptr2">The pointer to compare.</param>
    /// <returns>true if the value of the pointer is equal to the value of the specified pointer; otherwise, false.</returns>
    public static bool Equals(this nint left, nint ptr2) {
        return left == ptr2;
    }

    /// <summary>
    /// Determines whether the value of the pointer is equal to the specified 32-bit unsigned integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to compare.</param>
    /// <returns>true if the value of the pointer is equal to the value; otherwise, false.</returns>
    public static bool Equals(this nint pointer, uint value) {
        return pointer.ToUInt32() == value;
    }

    /// <summary>
    /// Determines whether the value of the pointer is equal to the specified 64-bit unsigned integer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The value to compare.</param>
    /// <returns>true if the value of the pointer is equal to the value; otherwise, false.</returns>
    public static bool Equals(this nint pointer, ulong value) {
        return pointer.ToUInt64() == value;
    }

    /// <summary>
    /// Determines whether the value of the pointer is greater than or equal to the value of the specified pointer.
    /// </summary>
    /// <param name="left">The pointer.</param>
    /// <param name="right">The pointer to compare.</param>
    /// <returns>true if the value of the pointer is greater than or equal to the value of the specified pointer; otherwise, false.</returns>
    public static bool GreaterThanOrEqualTo(this nint left, nint right) {
        return left.CompareTo(right) >= 0;
    }

    /// <summary>
    /// Determines whether the value of the pointer is less than or equal to the value of the specified pointer.
    /// </summary>
    /// <param name="left">The pointer.</param>
    /// <param name="right">The pointer to compare.</param>
    /// <returns>true if the value of the pointer is less than or equal to the value of the specified pointer; otherwise, false.</returns>
    public static bool LessThanOrEqualTo(this nint left, nint right) {
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
    public static nint And(this nint pointer, nint value) {
        switch (nint.Size) {
            case sizeof(int):
                return new nint(pointer.ToInt32() & value.ToInt32());

            default:
                return new nint(pointer.ToInt64() & value.ToInt64());
        }
    }

    /// <summary>
    /// Performs a bitwise NOT operation on the value of the pointer.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <returns>A new pointer that represents the result of the bitwise NOT operation.</returns>
    public static nint Not(this nint pointer) {
        switch (nint.Size) {
            case sizeof(int):
                return new nint(~pointer.ToInt32());

            default:
                return new nint(~pointer.ToInt64());
        }
    }

    /// <summary>
    /// Performs a bitwise OR operation on the value of the pointer and the specified pointer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The pointer value to perform the OR operation with.</param>
    /// <returns>A new pointer that represents the result of the bitwise OR operation.</returns>
    public static nint Or(this nint pointer, nint value) {
        switch (nint.Size) {
            case sizeof(int):
                return new nint(pointer.ToInt32() | value.ToInt32());

            default:
                return new nint(pointer.ToInt64() | value.ToInt64());
        }
    }

    /// <summary>
    /// Performs a bitwise XOR operation on the value of the pointer and the specified pointer value.
    /// </summary>
    /// <param name="pointer">The pointer.</param>
    /// <param name="value">The pointer value to perform the XOR operation with.</param>
    /// <returns>A new pointer that represents the result of the bitwise XOR operation.</returns>
    public static nint Xor(this nint pointer, nint value) {
        switch (nint.Size) {
            case sizeof(int):
                return new nint(pointer.ToInt32() ^ value.ToInt32());

            default:
                return new nint(pointer.ToInt64() ^ value.ToInt64());
        }
    }

    #endregion
}
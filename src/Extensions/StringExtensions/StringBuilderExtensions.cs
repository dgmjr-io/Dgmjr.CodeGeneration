//
// StringBuilderExtensions.cs
//
//   Created: 2022-10-30-10:35:49
//   Modified: 2022-10-30-10:42:27
//
//   Author: David G. Moore, Jr. <david@dgmjr.io>
//
//   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
//

// ReSharper disable once CheckNamespace
namespace System.Text;

public class IndentedStringBuilder : IStringBuilder
{
    private readonly StringBuilder _sb = new();
    private int _indent = 0;

    public virtual string Indent => new(' ', _indent);

    public int Capacity
    {
        get => _sb.Capacity;
        set => _sb.Capacity = value;
    }
    public int Length
    {
        get => _sb.Length;
        set => _sb.Length = value;
    }

    public int MaxCapacity => _sb.MaxCapacity;

    public char this[int index]
    {
        get => _sb[index];
        set => _sb[index] = value;
    }

    public IndentedStringBuilder() : this(new StringBuilder()) { }

    public IndentedStringBuilder(StringBuilder sb)
    {
        _sb = sb;
    }

    public static implicit operator IndentedStringBuilder(StringBuilder sb) => new(sb);

    public static IndentedStringBuilder operator ++(IndentedStringBuilder sb)
    {
        sb._indent++;
        return sb;
    }

    public static IndentedStringBuilder operator --(IndentedStringBuilder sb)
    {
        sb._indent--;
        return sb;
    }

    public IndentedStringBuilder AppendLine(string value, string prefix = "")
    {
        return AppendLine($"{prefix}{value}");
    }

    public IndentedStringBuilder AppendLines(IEnumerable<string> values, string prefix = "")
    {
        return AppendLine(string.Join($"{prefix}\r\n", values));
    }

    public StringBuilder Append(bool value) => _sb.Append(value);

    public StringBuilder Append(byte value) => _sb.Append(value);

    public StringBuilder Append(char value) => _sb.Append(value);

    public unsafe StringBuilder Append(char* value, int valueCount) =>
        _sb.Append(value, valueCount);

    public StringBuilder Append(char value, int repeatCount) => _sb.Append(value, repeatCount);

    public StringBuilder Append(char[] value) => _sb.Append(value);

    public StringBuilder Append(char[] value, int startIndex, int charCount) =>
        _sb.Append(value, startIndex, charCount);

    public StringBuilder Append(decimal value) => _sb.Append(value);

    public StringBuilder Append(double value) => _sb.Append(value);

    public StringBuilder Append(short value) => _sb.Append(value);

    public StringBuilder Append(int value) => _sb.Append(value);

    public StringBuilder Append(long value) => _sb.Append(value);

    public StringBuilder Append(object value) => _sb.Append(value);

    public StringBuilder Append(sbyte value) => _sb.Append(value);

    public StringBuilder Append(float value) => _sb.Append(value);

    public StringBuilder Append(string value) => _sb.Append(value);

    public StringBuilder Append(string value, int startIndex, int count) =>
        _sb.Append(value, startIndex, count);

    public StringBuilder Append(ushort value) => _sb.Append(value);

    public StringBuilder Append(uint value) => _sb.Append(value);

    public StringBuilder Append(ulong value) => _sb.Append(value);

    public StringBuilder AppendFormat(IFormatProvider provider, string format, object arg0) =>
        _sb.AppendFormat(provider, format, arg0);

    public StringBuilder AppendFormat(
        IFormatProvider provider,
        string format,
        object arg0,
        object arg1
    ) => _sb.AppendFormat(provider, format, arg0, arg1);

    public StringBuilder AppendFormat(
        IFormatProvider provider,
        string format,
        object arg0,
        object arg1,
        object arg2
    ) => _sb.AppendFormat(provider, format, arg0, arg1, arg2);

    public StringBuilder AppendFormat(IFormatProvider provider, string format, object[] args) =>
        _sb.AppendFormat(provider, format, args);

    public StringBuilder AppendFormat(string format, object arg0) => _sb.AppendFormat(format, arg0);

    public StringBuilder AppendFormat(string format, object arg0, object arg1) =>
        _sb.AppendFormat(format, arg0, arg1);

    public StringBuilder AppendFormat(string format, object arg0, object arg1, object arg2) =>
        _sb.AppendFormat(format, arg0, arg1, arg2);

    public StringBuilder AppendFormat(string format, object[] args) =>
        _sb.AppendFormat(format, args);

    public StringBuilder AppendLine() => _sb.AppendLine();

    public StringBuilder AppendLine(string value) => _sb.AppendLine(value);

    public StringBuilder Clear() => _sb.Clear();

    public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) =>
        _sb.CopyTo(sourceIndex, destination, destinationIndex, count);

    public int EnsureCapacity(int capacity) => _sb.EnsureCapacity(capacity);

    public bool Equals(StringBuilder sb) => _sb.Equals(sb);

    public StringBuilder Insert(int index, bool value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, byte value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, char value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, char[] value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, char[] value, int startIndex, int charCount) =>
        _sb.Insert(index, value, startIndex, charCount);

    public StringBuilder Insert(int index, decimal value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, double value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, short value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, int value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, long value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, object value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, sbyte value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, float value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, string value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, string value, int count) =>
        _sb.Insert(index, value, count);

    public StringBuilder Insert(int index, ushort value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, uint value) => _sb.Insert(index, value);

    public StringBuilder Insert(int index, ulong value) => _sb.Insert(index, value);

    public StringBuilder Remove(int startIndex, int length) => _sb.Remove(startIndex, length);

    public StringBuilder Replace(char oldChar, char newChar) => _sb.Replace(oldChar, newChar);

    public StringBuilder Replace(char oldChar, char newChar, int startIndex, int count) =>
        _sb.Replace(oldChar, newChar, startIndex, count);

    public StringBuilder Replace(string oldValue, string newValue) =>
        _sb.Replace(oldValue, newValue);

    public StringBuilder Replace(string oldValue, string newValue, int startIndex, int count) =>
        _sb.Replace(oldValue, newValue, startIndex, count);

    public string ToString(int startIndex, int length) => _sb.ToString(startIndex, length);
}

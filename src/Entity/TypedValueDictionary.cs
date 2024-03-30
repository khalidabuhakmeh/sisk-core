﻿// The Sisk Framework source code
// Copyright (c) 2023 PROJECT PRINCIPIUM
//
// The code below is licensed under the MIT license as
// of the date of its publication, available at
//
// File name:   TypedValueDictionary.cs
// Repository:  https://github.com/sisk-http/core

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Sisk.Core.Entity;

/// <summary>
/// Represents the base class for storing and retriving data by their type fluent
/// methods.
/// </summary>
/// <definition>
/// public class TypedBagRepository : IDictionary{{string, object?}}
/// </definition>
/// <type>
/// Class
/// </type>
public class TypedValueDictionary : IDictionary<string, object?>
{
    readonly Dictionary<string, object?> _values;

    /// <summary>
    /// Creates an new <see cref="TypedValueDictionary"/> instance with default parameters.
    /// </summary>
    /// <definition>
    /// public TypedBagRepository()
    /// </definition>
    /// <type>
    /// Constructor
    /// </type>
    public TypedValueDictionary()
    {
        _values = new();
    }

    /// <summary>
    /// Creates an new <see cref="TypedValueDictionary"/> instance with default parameters with the specified
    /// <see cref="StringComparer"/>.
    /// </summary>
    /// <definition>
    /// public TypedBagRepository(StringComparer keyComparer)
    /// </definition>
    /// <type>
    /// Constructor
    /// </type>
    public TypedValueDictionary(StringComparer keyComparer)
    {
        _values = new Dictionary<string, object?>(keyComparer);
    }

    /// <summary>
    /// Gets the Type full qualified key name.
    /// </summary>
    /// <param name="t">The type to get their qualified key name.</param>
    /// <definition>
    /// protected string GetTypeKeyName(Type t)
    /// </definition>
    /// <type>
    /// Protected method
    /// </type>
    protected string GetTypeKeyName(Type t) =>
        t.FullName + "+" + t.GetHashCode();

    /// <summary>
    /// Determines whether the specified <typeparamref name="T"/> singleton is defined in this context.
    /// </summary>
    /// <typeparam name="T">The singleton type.</typeparam>
    /// <definition>
    /// public bool IsSet{{T}}() where T : notnull
    /// </definition>
    /// <type>
    /// Method
    /// </type>
    public bool IsSet<T>() where T : notnull
    {
        return _values.ContainsKey(GetTypeKeyName(typeof(T)));
    }

    /// <summary>
    /// Determines whether the specified <typeparamref name="T"/> singleton is defined in this context and tries to
    /// output it.
    /// </summary>
    /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns>true if the object is find with the specified key; otherwise, false.</returns>
    /// <typeparam name="T">The singleton type.</typeparam>
    /// <definition>
    /// public bool IsSet{{T}}([NotNullWhen(true)] out T value) where T : notnull
    /// </definition>
    /// <type>
    /// Method
    /// </type>
    public bool IsSet<T>([NotNullWhen(true)] out T? value) where T : notnull
    {
        return TryGetValue(GetTypeKeyName(typeof(T)), out value);
    }

    /// <summary>
    /// Removes an singleton object from it's type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The singleton type.</typeparam>
    /// <definition>
    /// public void Unset{{T}}() where T : notnull
    /// </definition>
    /// <type>
    /// Method
    /// </type>
    public void Unset<T>() where T : notnull
    {
        _values.Remove(GetTypeKeyName(typeof(T)));
    }

    /// <summary>
    /// Creates and adds an singleton of <typeparamref name="T"/> in this context bag.
    /// </summary>
    /// <typeparam name="T">The object that will be defined in this context bag.</typeparam>
    /// <definition>
    /// public T Set{{T}}() where T : notnull, new()
    /// </definition>
    /// <type>
    /// Method
    /// </type>
    public T Set<T>() where T : notnull, new()
    {
        return Set<T>(new T());
    }

    /// <summary>
    /// Adds an singleton of <typeparamref name="T"/> in this context bag.
    /// </summary>
    /// <typeparam name="T">The object that will be defined in this context bag.</typeparam>
    /// <param name="value">The instance of <typeparamref name="T"/> which will be defined in this context bag.</param>
    /// <definition>
    /// public T Set{{T}}(T value) where T : notnull
    /// </definition>
    /// <type>
    /// Method
    /// </type>
    public T Set<T>(T value) where T : notnull
    {
        Type contextType = typeof(T);
        _values[GetTypeKeyName(contextType)] = value;
        return value;
    }

    /// <summary>
    /// Gets a singleton previously defined in this context bag via it's type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object defined in this context bag.</typeparam>
    /// <definition>
    /// public T Get{{T}}() where T : notnull
    /// </definition>
    /// <type>
    /// Method
    /// </type>
    public T Get<T>() where T : notnull
    {
        Type contextType = typeof(T);
        string key = GetTypeKeyName(contextType);
        if (ContainsKey(key))
        {
            return (T)_values[key]!;
        }
        else
        {
            throw new ArgumentException(string.Format(SR.HttpContextBagRepository_UndefinedDynamicProperty, contextType.FullName));
        }
    }

    /// <inheritdoc />
    /// <nodoc />
    public object? this[string key] { get => _values[key]; set => _values[key] = value; }

    /// <inheritdoc />
    /// <nodoc />
    public ICollection<string> Keys => _values.Keys;

    /// <inheritdoc />
    /// <nodoc />
    public ICollection<object?> Values => _values.Values;

    /// <inheritdoc />
    /// <nodoc />
    public int Count => _values.Count;

    /// <inheritdoc />
    /// <nodoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    /// <nodoc />
    public void Add(string key, object? value)
    {
        _values.Add(key, value);
    }

    /// <inheritdoc />
    /// <nodoc />
    public void Add(KeyValuePair<string, object?> item)
    {
        _values.Add(item.Key, item.Value);
    }

    /// <inheritdoc />
    /// <nodoc />
    public void Clear()
    {
        _values.Clear();
    }

    /// <inheritdoc />
    /// <nodoc />
    public bool Contains(KeyValuePair<string, object?> item)
    {
        return _values.Contains(item);
    }

    /// <inheritdoc />
    /// <nodoc />
    public bool ContainsKey(string key)
    {
        return _values.ContainsKey(key);
    }

    /// <inheritdoc />
    /// <nodoc />
    public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<string, object?>>)_values).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    /// <nodoc />
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    /// <inheritdoc />
    /// <nodoc />
    public bool Remove(string key)
    {
        return _values.Remove(key);
    }

    /// <inheritdoc />
    /// <nodoc />
    public bool Remove(KeyValuePair<string, object?> item)
    {
        return ((ICollection<KeyValuePair<string, object?>>)_values).Remove(item);
    }

    /// <inheritdoc />
    /// <nodoc />
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object? value)
    {
        return _values.TryGetValue(key, out value);
    }

    /// <summary>
    /// Gets the value associated with the specified key and casts it into <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">The type which will be casted into.</typeparam>
    /// <param name="key">The key whose to get.</param>
    /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns>true if the object is find with the specified key; otherwise, false.</returns>
    /// <definition>
    /// public Boolean TryGetValue{{TResult}}(String key, [MaybeNullWhen(false)] out TResult? value)
    /// </definition>
    /// <type>
    /// Method
    /// </type>
    public bool TryGetValue<TResult>(string key, [MaybeNullWhen(false)] out TResult? value)
    {
        bool b = _values.TryGetValue(key, out var v);
        if (b)
        {
            value = (TResult?)v;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    /// <inheritdoc />
    /// <nodoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_values).GetEnumerator();
    }
}

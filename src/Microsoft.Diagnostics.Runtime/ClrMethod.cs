﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Microsoft.Diagnostics.Runtime
{
    /// <summary>
    /// Represents a method on a class.
    /// </summary>
    public abstract class ClrMethod
    {
        /// <summary>
        /// Retrieves the first MethodDesc in EnumerateMethodDescs().  For single
        /// AppDomain programs this is the only MethodDesc.  MethodDescs
        /// are unique to an Method/AppDomain pair, so when there are multiple domains
        /// there will be multiple MethodDescs for a method.
        /// </summary>
        abstract public ulong MethodDesc { get; }

        /// <summary>
        /// Enumerates all method descs for this method in the process.  MethodDescs
        /// are unique to an Method/AppDomain pair, so when there are multiple domains
        /// there will be multiple MethodDescs for a method.
        /// </summary>
        /// <returns>An enumeration of method handles in the process for this given
        /// method.</returns>
        abstract public IEnumerable<ulong> EnumerateMethodDescs();

        /// <summary>
        /// The name of the method.  For example, "void System.Foo.Bar(object o, int i)" would return "Bar".
        /// </summary>
        abstract public string Name { get; }

        /// <summary>
        /// Returns the full signature of the function.  For example, "void System.Foo.Bar(object o, int i)"
        /// would return "System.Foo.Bar(System.Object, System.Int32)"
        /// </summary>
        abstract public string GetFullSignature();

        /// <summary>
        /// Returns the instruction pointer in the target process for the start of the method's assembly.
        /// </summary>
        abstract public ulong NativeCode { get; }

        /// <summary>
        /// Gets the ILOffset of the given address within this method.
        /// </summary>
        /// <param name="addr">The absolute address of the code (not a relative offset).</param>
        /// <returns>The IL offset of the given address.</returns>
        abstract public int GetILOffset(ulong addr);

        /// <summary>
        /// Returns the location in memory of the IL for this method.
        /// </summary>
        abstract public ILInfo IL { get; }
        
        /// <summary>
        /// Returns the regions of memory that 
        /// </summary>
        abstract public HotColdRegions HotColdInfo { get; }

        /// <summary>
        /// Returns the way this method was compiled.
        /// </summary>
        abstract public MethodCompilationType CompilationType { get; }

        /// <summary>
        /// Returns the IL to native offset mapping.
        /// </summary>
        abstract public ILToNativeMap[] ILOffsetMap { get; }

        /// <summary>
        /// Returns the metadata token of the current method.
        /// </summary>
        abstract public uint MetadataToken { get; }

        /// <summary>
        /// Returns the enclosing type of this method.
        /// </summary>
        abstract public ClrType Type { get; }

        // Visibility:
        /// <summary>
        /// Returns if this method is public.
        /// </summary>
        abstract public bool IsPublic { get; }

        /// <summary>
        /// Returns if this method is private.
        /// </summary>
        abstract public bool IsPrivate { get; }

        /// <summary>
        /// Returns if this method is internal.
        /// </summary>
        abstract public bool IsInternal { get; }

        /// <summary>
        /// Returns if this method is protected.
        /// </summary>
        abstract public bool IsProtected { get; }

        // Attributes:
        /// <summary>
        /// Returns if this method is static.
        /// </summary>
        abstract public bool IsStatic { get; }
        /// <summary>
        /// Returns if this method is final.
        /// </summary>
        abstract public bool IsFinal { get; }
        /// <summary>
        /// Returns if this method is a PInvoke.
        /// </summary>
        abstract public bool IsPInvoke { get; }
        /// <summary>
        /// Returns if this method is a special method.
        /// </summary>
        abstract public bool IsSpecialName { get; }
        /// <summary>
        /// Returns if this method is runtime special method.
        /// </summary>
        abstract public bool IsRTSpecialName { get; }

        /// <summary>
        /// Returns if this method is virtual.
        /// </summary>
        abstract public bool IsVirtual { get; }
        /// <summary>
        /// Returns if this method is abstract.
        /// </summary>
        abstract public bool IsAbstract { get; }

        /// <summary>
        /// Returns the location of the GCInfo for this method.
        /// </summary>
        abstract public ulong GCInfo { get; }

        /// <summary>
        /// Returns whether this method is an instance constructor.
        /// </summary>
        virtual public bool IsConstructor { get { return Name == ".ctor"; } }

        /// <summary>
        /// Returns whether this method is a static constructor.
        /// </summary>
        virtual public bool IsClassConstructor { get { return Name == ".cctor"; } }
    }

}

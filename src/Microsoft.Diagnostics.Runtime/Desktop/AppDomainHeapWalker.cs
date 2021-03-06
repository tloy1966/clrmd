﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Diagnostics.Runtime.DacInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Diagnostics.Runtime.Desktop
{
    internal class AppDomainHeapWalker
    {
        #region Variables
        private enum InternalHeapTypes
        {
            IndcellHeap,
            LookupHeap,
            ResolveHeap,
            DispatchHeap,
            CacheEntryHeap
        }

        private List<MemoryRegion> _regions = new List<MemoryRegion>();
        private SOSDac.LoaderHeapTraverse _delegate;
        private ClrMemoryRegionType _type;
        private ulong _appDomain;
        private DesktopRuntimeBase _runtime;
        #endregion

        public AppDomainHeapWalker(DesktopRuntimeBase runtime)
        {
            _runtime = runtime;
            _delegate = new SOSDac.LoaderHeapTraverse(VisitOneHeap);
        }

        public IEnumerable<MemoryRegion> EnumerateHeaps(IAppDomainData appDomain)
        {
            Debug.Assert(appDomain != null);
            _appDomain = appDomain.Address;
            _regions.Clear();

            // Standard heaps.
            _type = ClrMemoryRegionType.LowFrequencyLoaderHeap;
            _runtime.TraverseHeap(appDomain.LowFrequencyHeap, _delegate);

            _type = ClrMemoryRegionType.HighFrequencyLoaderHeap;
            _runtime.TraverseHeap(appDomain.HighFrequencyHeap, _delegate);

            _type = ClrMemoryRegionType.StubHeap;
            _runtime.TraverseHeap(appDomain.StubHeap, _delegate);

            // Stub heaps.
            _type = ClrMemoryRegionType.IndcellHeap;
            _runtime.TraverseStubHeap(_appDomain, (int)InternalHeapTypes.IndcellHeap, _delegate);

            _type = ClrMemoryRegionType.LookupHeap;
            _runtime.TraverseStubHeap(_appDomain, (int)InternalHeapTypes.LookupHeap, _delegate);

            _type = ClrMemoryRegionType.ResolveHeap;
            _runtime.TraverseStubHeap(_appDomain, (int)InternalHeapTypes.ResolveHeap, _delegate);

            _type = ClrMemoryRegionType.DispatchHeap;
            _runtime.TraverseStubHeap(_appDomain, (int)InternalHeapTypes.DispatchHeap, _delegate);

            _type = ClrMemoryRegionType.CacheEntryHeap;
            _runtime.TraverseStubHeap(_appDomain, (int)InternalHeapTypes.CacheEntryHeap, _delegate);

            return _regions;
        }

        public IEnumerable<MemoryRegion> EnumerateModuleHeaps(IAppDomainData appDomain, ulong addr)
        {
            Debug.Assert(appDomain != null);
            _appDomain = appDomain.Address;
            _regions.Clear();

            if (addr == 0)
                return _regions;

            IModuleData module = _runtime.GetModuleData(addr);
            if (module != null)
            {
                _type = ClrMemoryRegionType.ModuleThunkHeap;
                _runtime.TraverseHeap(module.ThunkHeap, _delegate);

                _type = ClrMemoryRegionType.ModuleLookupTableHeap;
                _runtime.TraverseHeap(module.LookupTableHeap, _delegate);
            }

            return _regions;
        }

        public IEnumerable<MemoryRegion> EnumerateJitHeap(ulong heap)
        {
            _appDomain = 0;
            _regions.Clear();

            _type = ClrMemoryRegionType.JitLoaderCodeHeap;
            _runtime.TraverseHeap(heap, _delegate);

            return _regions;
        }

        #region Helper Functions
        private void VisitOneHeap(ulong address, IntPtr size, int isCurrent)
        {
            if (_appDomain == 0)
                _regions.Add(new MemoryRegion(_runtime, address, (ulong)size.ToInt64(), _type));
            else
                _regions.Add(new MemoryRegion(_runtime, address, (ulong)size.ToInt64(), _type, _appDomain));
        }
        #endregion

    }
}

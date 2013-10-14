// Guids.cs
// MUST match guids.h
using System;

namespace KAVE.KAVE_MessageBus
{
    static class GuidList
    {
        public const string guidKAVE_MessageBusPkgString = "83caccc0-f401-40e7-9a8f-1c02294f6209";
        public const string guidKAVE_MessageBusCmdSetString = "c392502e-4839-4415-811c-bec2ee553209";

        public static readonly Guid guidKAVE_MessageBusCmdSet = new Guid(guidKAVE_MessageBusCmdSetString);
    };
}
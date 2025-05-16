namespace Ara3D.Utils
{
    public static class HResult
    {
        //==================================================
        // ABOUT HRESULT:
        //==================================================
        //
        // From: https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-erref/1bc92ddf-b79e-413c-bbaa-99a5281a6c90?redirectedfrom=MSDN
        // See also: https://docs.microsoft.com/en-us/windows/win32/seccrypto/common-hresult-values
        //
        // The HRESULT numbering space is vendor-extensible. Vendors can supply their own values for this
        // field, as long as the C bit(0x20000000) is set, indicating it is a customer code.
        //
        // The HRESULT numbering space has the following internal structure. Any protocol that uses NTSTATUS
        // values on the wire is responsible for stating the order in which the bytes are placed on the wire.
        //
        // |                                         1                                       2                                       3     |
        // | 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 0 | 1 |
        // | S | R | C | N | X |                 Facility                  |                           Code                                |
        // 
        // S (1 bit): Severity. If set, indicates a failure result. If clear, indicates a success result.
        //
        // R (1 bit): Reserved. If the N bit is clear, this bit MUST be set to 0. If the N bit is set, this bit is
        //                      defined by the NTSTATUS numbering space(as specified in section 2.3).
        //
        // C (1 bit): Customer. This bit specifies if the value is customer-defined or Microsoft-defined. The bit is
        //                      set for customer-defined values and clear for Microsoft-defined values.
        //
        // N (1 bit): If set, indicates that the error code is an NTSTATUS value (as specified in section 2.3),
        //            except that this bit is set.
        //
        // X (1 bit): Reserved. Should be set to 0.
        //
        // Facility (11 bits): An indicator of the source of the error. New facilities are occasionally added by
        //                     Microsoft.
        //
        // Code (2 bytes): The remainder of the error code.

        /// <summary>
        /// The failure severity HResult mask.
        /// </summary>
        public const uint SeverityFailureMask = 0x80000000;

        /// <summary>
        /// The custom HResult mask.
        /// </summary>
        public const uint CustomMask = 0x20000000;

        /// <summary>
        /// The custom failure HResult mask. <br/>
        /// Signed decimal: -1610612736 <br/>
        /// Unsigned decimal: 2684354560 <br/>
        /// Hex: 0xA0000000 <br/>
        /// </summary>
        public const uint CustomFailureMask = SeverityFailureMask | CustomMask;

        //==================================================
        // COMMON HRESULT VALUES
        //==================================================

        /// <summary>
        /// Success
        /// </summary>
        public const int Success = 0x0;

        /// <summary>
        /// Unspecified failure
        /// </summary>
        public const int Failure = unchecked((int)0x80004005);

        /// <summary>
        /// Command line usage error
        /// </summary>
        public const int UsageError = unchecked((int)0x80070057);

        /// <summary>
        /// File not found error.
        /// </summary>
        public const int FileNotFound = unchecked((int)0x80070002);
    }
}
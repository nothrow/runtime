// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography;
using Internal.NativeCrypto;

namespace Internal.Cryptography
{
    internal static class TripleDesBCryptModes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5350")] // We are providing the implementation for 3DES not consuming it
        private static readonly SafeAlgorithmHandle s_hAlgCbc = Open3DesAlgorithm(Cng.BCRYPT_CHAIN_MODE_CBC);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5350")] // We are providing the implementation for 3DES not consuming it
        private static readonly SafeAlgorithmHandle s_hAlgEcb = Open3DesAlgorithm(Cng.BCRYPT_CHAIN_MODE_ECB);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5350")] // We are providing the implementation for 3DES not consuming it
        private static readonly SafeAlgorithmHandle s_hAlgCfb8 = Open3DesAlgorithm(Cng.BCRYPT_CHAIN_MODE_CFB, 1);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5350")] // We are providing the implementation for 3DES not consuming it
        private static readonly SafeAlgorithmHandle s_hAlgCfb64 = Open3DesAlgorithm(Cng.BCRYPT_CHAIN_MODE_CFB, 8);

        internal static SafeAlgorithmHandle GetSharedHandle(CipherMode cipherMode, int feedback) =>
            // Windows 8 added support to set the CipherMode value on a key,
            // but Windows 7 requires that it be set on the algorithm before key creation.
            (cipherMode, feedback) switch
            {
                (CipherMode.CBC, _) => s_hAlgCbc,
                (CipherMode.ECB, _) => s_hAlgEcb,
                (CipherMode.CFB, 1) => s_hAlgCfb8,
                (CipherMode.CFB, 8) => s_hAlgCfb64,
                _ => throw new NotSupportedException(),
            };

        private static SafeAlgorithmHandle Open3DesAlgorithm(string cipherMode, int feedback = 0)
        {
            SafeAlgorithmHandle hAlg = Cng.BCryptOpenAlgorithmProvider(Cng.BCRYPT_3DES_ALGORITHM, null, Cng.OpenAlgorithmProviderFlags.NONE);
            hAlg.SetCipherMode(cipherMode);

            if (feedback > 0)
            {
                // feedback is in bytes!
                hAlg.SetFeedbackSize(feedback);
            }

            return hAlg;
        }
    }
}

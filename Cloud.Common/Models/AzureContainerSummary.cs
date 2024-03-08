// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Azure.Storage.Blobs.Models;
using Econolite.Ode.Extensions;

namespace Econolite.Ode.Cloud.Common.Models
{
    public class AzureContainerSummary
    {
        /// <summary>
        /// The total number of blobs in the container
        /// </summary>
        public long BlobCount { get; set; }

        /// <summary>
        /// The total size in bytes for the given container
        /// </summary>
        public long ContainerByteSize { get; set; }

        /// <summary>
        /// Converts the byte size to a string format B, KB, MB, GB, TB, PB, EB
        /// </summary>
        public string ContainerSize
        {
            get
            {
                return this.ContainerByteSize.AdaptByteSizeToString();
            }
        }

        /// <summary>
        /// The blobs inside the container
        /// </summary>
        public IEnumerable<BlobItem> Blobs { get; set; } = new List<BlobItem>();
    }
}
